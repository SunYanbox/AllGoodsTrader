using System.Reflection;
using AllGoodsTrader.Configs;
using AllGoodsTrader.Services;
using AllGoodsTrader.SptHelpers;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Utils.Json;

namespace AllGoodsTrader.Models;

[Injectable(InjectionType.Singleton, TypePriority = OnLoadOrder.TraderRegistration + 1)]
public class TraderFactory(
    TimeUtil timeUtil,
    ModHelper modHelper,
    ItemHelper itemHelper,
    ImageRouter imageRouter,
    ConfigServer configServer,
    DatabaseServer databaseService,
    ISptLogger<TraderFactory> logger,
    ModConfigService modConfigService,
    ItemCategoryService itemCategoryService,
    FluentTraderAssortCreator assortCreator,
    AddCustomTraderHelper addCustomTraderHelper
    ): IOnLoad
{
    private readonly TraderConfig _traderConfig = configServer.GetConfig<TraderConfig>();
    private readonly RagfairConfig _ragfairConfig = configServer.GetConfig<RagfairConfig>();
    private string PathToMod => modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
    private readonly string RoublesString = Money.ROUBLES.ToString();
    
    public static readonly List<MongoId> SecureContainerIds =
    [
        ItemTpl.SECURE_CONTAINER_ALPHA,
        ItemTpl.SECURE_CONTAINER_BETA,
        ItemTpl.SECURE_CONTAINER_BOSS,
        ItemTpl.SECURE_CONTAINER_EPSILON,
        ItemTpl.SECURE_CONTAINER_GAMMA,
        ItemTpl.SECURE_CONTAINER_GAMMA_TUE,
        ItemTpl.SECURE_CONTAINER_KAPPA,
        ItemTpl.SECURE_CONTAINER_KAPPA_DESECRATED,
        ItemTpl.SECURE_CONTAINER_THETA,
        ItemTpl.SECURE_DEVELOPER_SECURE_CONTAINER,
        ItemTpl.SECURE_TOURNAMENT_SECURED_CONTAINER,
        ItemTpl.SECURE_WAIST_POUCH
    ];

    public Task OnLoad()
    {
        foreach (TraderData traderData in TraderConfigs.AllTraderConfigs)
        {
            try
            {
                ModTraderConfig modTraderConfig = modConfigService.Traders[traderData.Id];
                if (!modTraderConfig.Enabled)
                {
                    continue;
                }
                AddTrader(modTraderConfig.AsTraderBase(traderData), traderData);
            }
            catch (Exception e)
            {
                logger.Error($"[AllGoodsTrader] Add Trader({traderData.Id}) Error:", e);
            }
        }
        logger.Info("[AllGoodsTrader] Mod Loaded");
        return Task.CompletedTask;
    }

    public void AddTrader(TraderBase traderBase, TraderData traderData)
    {
        // Create a helper class and use it to register our traders image/icon + set its stock refresh time
        imageRouter.AddRoute(traderBase.Avatar!.Replace(".png", ""), System.IO.Path.Combine(PathToMod, traderData.AvatarFilePath));
        addCustomTraderHelper.SetTraderUpdateTime(_traderConfig, traderBase, timeUtil.GetHoursAsSeconds(1), timeUtil.GetHoursAsSeconds(2));

        // Add our trader to the config list, this lets it be seen by the flea market
        _ragfairConfig.Traders.TryAdd(traderBase.Id, true);

        // Add our trader (with no items yet) to the server database
        // An 'assort' is the term used to describe the offers a trader sells, it has 3 parts to an assort
        // 1: The item
        // 2: The barter scheme, cost of the item (money or barter)
        // 3: The Loyalty level, what rep level is required to buy the item from trader
        addCustomTraderHelper.AddTraderWithEmptyAssortToDb(traderBase);

        // Add localization text for our trader to the database so it shows to people playing in different languages
        AddTraderToLocales(traderData);

        
        List<TemplateItem> itemsToAdd = itemCategoryService.GetItemTemplate(traderData.BaseClasses);

        long successSecureCount = 0;
        if (traderData.Id == TraderConfigs.MiscTrader.Id)
        {
            foreach (MongoId secureContainerId in SecureContainerIds)
            {
                if (databaseService.GetTables().Templates
                        .Items.TryGetValue(secureContainerId, out TemplateItem? templateItem))
                {
                    itemsToAdd.Add(templateItem);
                    successSecureCount++;
                }
            }
            logger.Debug($"成功添加安全箱容器到Trader({traderBase.Id}){successSecureCount}个, 成功率: {(successSecureCount) /SecureContainerIds.Count:P2}");
        }

        List<Item> complexItems = [];

        long successSpecialItems = 0;
        long successHasSlotsItems = 0;
        long successCommonItems = 0;
        
        foreach (TemplateItem templateItem in itemsToAdd)
        {
            complexItems.Clear();
            complexItems.Add(new Item
            {
                Id = new MongoId(),
                Template = templateItem.Id,
                ParentId = DefaultParentIdAndSlotId,
                SlotId = DefaultParentIdAndSlotId
            });
            if (HandleSpecialItems(complexItems, templateItem))
            {
                successSpecialItems++;
                assortCreator.Export(traderBase.Id);
                continue;
            }
            
            if (itemHelper.ItemHasSlots(templateItem.Id))
            {
                itemHelper.AddChildSlotItems(complexItems, templateItem, requiredOnly: true);
                double price = 0;
                foreach (Item complexItem in complexItems)
                {
                    price += itemCategoryService.GetItemPrice(complexItem.Template);
                }
                if ((int)price <= 0)
                {
                    logger.Warning($"[AllGoodsTrader] Item(Id={templateItem.Id} with child) Price must be greater than 0. (Any child price == 0 or cant add child to root item)");
                    continue;
                }

                successHasSlotsItems++;
                assortCreator
                    .CreateComplexAssortItem(complexItems)
                    .AddUnlimitedStackCount()
                    .AddMoneyCost(Money.ROUBLES, (int)price)
                    .AddLoyaltyLevel(1)
                    .Export(traderBase.Id);
            }
            else
            {
                double price = itemCategoryService.GetItemPrice(templateItem.Id);
                if ((int)price <= 1e-3)
                {
                    logger.Debug($"[AllGoodsTrader] Item(Id={templateItem.Id}) Price must be greater than 0.");
                    continue;
                }

                successCommonItems++;
                assortCreator
                    .CreateSingleAssortItem(templateItem.Id)
                    .AddUnlimitedStackCount()
                    .AddMoneyCost(RoublesString, (int)price)
                    .AddLoyaltyLevel(1)
                    .Export(traderBase.Id);
            }
        }
        
        logger.Debug($"[AllGoodsTrader] 为商人Trader({traderBase.Id})添加物品成功率: " +
                     $"{(double)(successCommonItems + successHasSlotsItems + successSpecialItems) / itemsToAdd.Count:P4}\n" +
                     $"\t{{ 普通物品: ({successCommonItems}), 有槽位物品: {successHasSlotsItems}, 特殊物品: {successSpecialItems} }} / 总物品: {itemsToAdd.Count}");
    }

    /// <summary>
    /// 处理特殊插槽物品
    /// </summary>
    /// <param name="items">只有一个根物体的列表</param>
    /// <param name="templateItem">根物品模板</param>
    /// <returns>是否是特殊物品</returns>
    public bool HandleSpecialItems(List<Item> items, TemplateItem templateItem)
    {
        // 火箭筒
        if (templateItem.Id == ItemTpl.ROCKETLAUNCHER_RSHG2_725MM_ROCKET_LAUNCHER)
        {
            double priceRocket725Shg2 = 
                itemCategoryService.GetItemPrice(ItemTpl.ROCKET_725_SHG2)
                + itemCategoryService.GetItemPrice(ItemTpl.ROCKETLAUNCHER_RSHG2_725MM_ROCKET_LAUNCHER);

            if ((int)priceRocket725Shg2 <= 0)
            {
                logger.Error("[AllGoodsTrader] ItemTpl.ROCKET_725_SHG2 with child price = 0");
                return false;
            }

            items.Add(new Item
            {
                Id = new MongoId(),
                Template = ItemTpl.ROCKET_725_SHG2,
                ParentId = items[0].Id.ToString(),
                SlotId = "patron_in_weapon"
            });
            
            assortCreator
                .CreateComplexAssortItem(items)
                .AddUnlimitedStackCount()
                .AddMoneyCost(Money.ROUBLES, (int)priceRocket725Shg2)
                .AddLoyaltyLevel(1);
            
            logger.Debug($"处理火箭发射器{templateItem.Id}结果: {items.Count == 2}");
            
            return true;
        }

        // 弹药盒
        if (itemHelper.IsOfBaseclass(templateItem.Id, BaseClasses.AMMO_BOX))
        {
            if (templateItem.Properties == null || templateItem.Properties.StackSlots == null)
                return false;
            var parentId = items[0].Id.ToString();
            double price = itemCategoryService.GetItemPrice(templateItem.Id);
            foreach (StackSlot stackSlot in templateItem.Properties.StackSlots)
            {
                if (stackSlot.Properties == null || stackSlot.Properties.Filters == null || stackSlot.MaxCount == null) continue;
                foreach (SlotFilter filter in stackSlot.Properties.Filters)
                {
                    if (filter.Filter == null) continue;
                    foreach (MongoId ammoTpl in filter.Filter)
                    {
                        price += itemCategoryService.GetItemPrice(ammoTpl);
                        var ammoInner = new Item
                        {
                            Id = new MongoId(),
                            Template = ammoTpl,
                            ParentId = parentId,
                            SlotId = "cartridges",
                            Location = 0,
                            Upd = new Upd
                            {
                                StackObjectsCount= stackSlot.MaxCount
                            },
                        };
                        // logger.Debug($"[AllGoodsTrader] 已添加弹药盒的弹药: {ammoInner}\n");
                        items.Add(ammoInner);
                        // logger.Info($"[AllGoodsTrader] 弹药盒的弹药ID(Tpl: {ammoTpl}, dynamicId: {ammodynamicId})\n\t在assort中的数量: {assort.Items.Count(x => x.Id == ammoInner.Id)}\n");
                    }
                }
            }
            
            if ((int)price <= 0)
            {
                logger.Error($"[AllGoodsTrader] AMMO_BOX({templateItem.Id}) with child price = 0");
                return false;
            }
            
            assortCreator
                .CreateComplexAssortItem(items)
                .AddUnlimitedStackCount()
                .AddMoneyCost(Money.ROUBLES, (int)price)
                .AddLoyaltyLevel(1);
            
            // 这一句不隐藏太卡了
            // logger.Debug($"处理弹药盒{templateItem.Id}结果: {items.Count == 2}"); // \n```json\n{jsonUtil.Serialize(items, true)}\n```
            
            return true;
        }

        return false;
    }
    
    public void AddTraderToLocales(TraderData traderData)
    {
        if (traderData.Locales.Count <= 0)
        {
            logger.Critical($"[AllGoodsTrader] No locales found for trader: {traderData.Id}");
            return;
        }
        Dictionary<string, LazyLoad<Dictionary<string, string>>> locales = databaseService.GetTables().Locales.Global;
        MongoId newTraderId = traderData.Id;
        foreach ((string localeKey, LazyLoad<Dictionary<string, string>> localeKvP) in locales)
        {
            // We have to add a transformer here, because locales are lazy loaded due to them taking up huge space in memory
            // The transformer will make sure that each time the locales are requested, the ones added below are included
            string localKey = traderData.Locales.ContainsKey(localeKey) ? localeKey : TraderData.DefaultLocale;

            if (!traderData.Locales.ContainsKey(localKey))
            {
                localKey = traderData.Locales.Keys.First();
            }
            
            TraderLocales traderLocales = traderData.Locales[localKey];
            
            localeKvP.AddTransformer(lazyloadedLocaleData =>
            {
                lazyloadedLocaleData!.Add($"{newTraderId} FullName", traderLocales.Name);
                lazyloadedLocaleData.Add($"{newTraderId} FirstName", traderLocales.Nickname);
                lazyloadedLocaleData.Add($"{newTraderId} Nickname", traderLocales.Nickname);
                lazyloadedLocaleData.Add($"{newTraderId} Location", traderLocales.Location);
                lazyloadedLocaleData.Add($"{newTraderId} Description", traderLocales.Description);
                return lazyloadedLocaleData;
            });
        }
    }

    private const string DefaultParentIdAndSlotId = "hideout";
}