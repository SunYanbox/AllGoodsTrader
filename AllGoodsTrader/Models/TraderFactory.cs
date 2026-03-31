using System.Reflection;
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
    ISptLogger<TraderFactory> logger,
    DatabaseServer databaseService,
    FluentTraderAssortCreator assortCreator,
    AddCustomTraderHelper addCustomTraderHelper,
    ItemCategoryService itemCategoryService
    ): IOnLoad
{
    private readonly TraderConfig _traderConfig = configServer.GetConfig<TraderConfig>();
    private readonly RagfairConfig _ragfairConfig = configServer.GetConfig<RagfairConfig>();
    private string PathToMod => modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
    private readonly string RoublesString = Money.ROUBLES.ToString();

    public Task OnLoad()
    {
        foreach (TraderData traderData in TraderConfigs.AllTraderConfigs)
        {
            try
            {
                AddTrader(traderData);
            }
            catch (Exception e)
            {
                logger.Error($"[AllGoodsTrader] Add Trader({traderData.Id}) Error:", e);
            }
        }
        logger.Info("[AllGoodsTrader] Mod Loaded");
        return Task.CompletedTask;
    }

    public void AddTrader(TraderData traderData)
    {
        TraderBase traderBase = CreateTraderBase(traderData, traderData.AvailableRepair, traderData.AvailableInsurance);
        
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

        
        List<TemplateItem> items = itemCategoryService.GetItemTemplate(traderData.BaseClasses);
        foreach (TemplateItem templateItem in items)
        {
            if (itemHelper.ItemHasSlots(templateItem.Id))
            {
                List<Item> complexItems = [
                    new()
                    {
                        Id = new MongoId(),
                        Template = templateItem.Id,
                        ParentId = DefaultParentIdAndSlotId,
                        SlotId = DefaultParentIdAndSlotId
                    }
                ];
                itemHelper.AddChildSlotItems(complexItems, templateItem, requiredOnly: true);
                double price = 0;
                foreach (Item complexItem in complexItems)
                {
                    price += itemCategoryService.GetItemPrice(complexItem.Template);
                }
                if ((int)price <= 0)
                {
                    logger.Debug($"[AllGoodsTrader] Item(Id={templateItem.Id} with child) Price must be greater than 0. (Any child price == 0 or cant add child to root item)");
                    continue;
                }
                
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
                assortCreator
                    .CreateSingleAssortItem(templateItem.Id)
                    .AddUnlimitedStackCount()
                    .AddMoneyCost(RoublesString, (int)price)
                    .AddLoyaltyLevel(1)
                    .Export(traderBase.Id);
            }
        }
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
    
    /// <summary>
    /// 创建商人基础配置
    /// </summary>
    private static TraderBase CreateTraderBase(TraderData traderData, 
        bool repair = false,
        bool insurance = false)
    {
        var traderBase = new TraderBase
        {
            Id = traderData.Id,
            Name = traderData.Locales[TraderData.DefaultLocale].Name,
            Nickname = traderData.Locales[TraderData.DefaultLocale].Nickname,
            Location = traderData.Locales[TraderData.DefaultLocale].Location,
            Avatar = $"/files/trader/avatar/{traderData.Avatar}",
            Currency = CurrencyType.RUB,
            UnlockedByDefault = true,
            AvailableInRaid = false,
            GridHeight = 120,
            BalanceRub = 7000_0000,
            BalanceDollar = 0,
            BalanceEuro = 0,
            BuyerUp = false,
            CustomizationSeller = false,
            Discount = 0,
            DiscountEnd = 0,
            IsAvailableInPVE = true,
            IsCanTransferItems = false,
            IsCanTransferItemsFromPve = false,
            Medic = false,
            NextResupply = 0,
            SellCategory = [],
            TransferableItems = new ItemBuyData
            {
                Category = [],
                IdList = []
            },
            ProhibitedTransferableItems = new ItemBuyData
            {
                Category = [],
                IdList = []
            },
            ProhibitedItemsSellModifier = 0,
            Surname = traderData.Locales[TraderData.DefaultLocale].Nickname,
            ItemsBuy = new ItemBuyData
            {
                Category =
                [
                    BaseClasses.ITEM
                ],
                IdList = []
            },
            ItemsBuyProhibited = new ItemBuyData
            {
                Category = [],
                IdList = []
            },
            Insurance = new TraderInsurance
            {
                Availability = false,
                ExcludedCategory = [],
                MaxReturnHour = 0,
                MaxStorageTime = 99,
                MinPayment = 0,
                MinReturnHour = 0
            },
            Repair = new TraderRepair
            {
                Availability = false,
                Currency = Money.ROUBLES,
                CurrencyCoefficient = 1,
                ExcludedCategory = [],
                ExcludedIdList = [],
                Quality = 0,
                PriceRate = 0.8
            },
            LoyaltyLevels =
            [
                new TraderLoyaltyLevel
                {
                    BuyPriceCoefficient = 30,
                    ExchangePriceCoefficient = 0,
                    HealPriceCoefficient = 0,
                    InsurancePriceCoefficient = 20,
                    MinLevel = 1,
                    MinSalesSum = 0,
                    MinStanding = 0,
                    RepairPriceCoefficient = 130
                }
            ]
        };

        if (repair)
        {
            traderBase.Repair.Availability = true;
        }

        if (insurance)
        {
            traderBase.Insurance.Availability = true;
        }

        return traderBase;
    }
}