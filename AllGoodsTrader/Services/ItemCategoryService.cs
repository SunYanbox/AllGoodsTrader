using System.Reflection;
using AllGoodsTrader.Configs;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Ragfair;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using Path = System.IO.Path;

namespace AllGoodsTrader.Services;

/// <summary>
/// 物品分类服务
/// 用于按分类筛选物品
/// </summary>
[Injectable(InjectionType.Singleton, TypePriority = OnLoadOrder.TraderRegistration + 1)]
public class ItemCategoryService(
    JsonUtil jsonUtil,
    ModHelper modHelper,
    ItemHelper itemHelper,
    DatabaseService databaseService,
    RagfairController ragfairController,
    ISptLogger<ItemCategoryService> logger)
{
    private string PathToMod => modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
    private string ConfigPath => Path.Combine(PathToMod, "data", "config.json");
    private ModConfig? _modConfig;
    
    /// <summary>
    /// 根据模组配置获取物品价格
    /// </summary>
    /// <param name="itemTplId">物品的模板Id</param>
    /// <returns></returns>
    public double GetItemPrice(MongoId itemTplId)
    {
        if (_modConfig is null)
        {
            try
            {
                _modConfig = jsonUtil.DeserializeFromFile<ModConfig>(ConfigPath);
            }
            catch (Exception e)
            {
                logger.Error($"加载配置文件\"{ConfigPath}\"失败, 将使用默认配置", e);
            }

            if (_modConfig is null)
            {
                _modConfig = new ModConfig();
                File.WriteAllText(ConfigPath, jsonUtil.Serialize(_modConfig, true));
            }
        }
        
        double? handbookPrice = itemHelper.GetItemPrice(itemTplId);
        double? ragfairPrice = ragfairController.GetItemMinAvgMaxFleaPriceValues(new GetMarketPriceRequestData
        {
            TemplateId = itemTplId
        }).Avg;
        
        double? basePrice = _modConfig.PriceMode switch
        {
            "Handbook" => handbookPrice,
            "AvgRagfair" => ragfairPrice,
            _ => GetMinValue(handbookPrice, ragfairPrice)  // 默认模式：取最小值
        };
        
        return (basePrice ?? 0) * (_modConfig.PriceModify ?? 1.0);
        
        // 获取两个可空值中的最小值，如果其中一个为 null 则返回另一个，都为 null 则返回 null
        double? GetMinValue(double? a, double? b)
        {
            return a switch
            {
                null when b == null => null,
                null => b,
                _ => b == null ? a : Math.Min(a.Value, b.Value)
            };
        }
    }

    /// <summary>
    /// 获取指定分类下的所有物品模板
    /// </summary>
    public List<TemplateItem> GetItemTemplate(List<MongoId> categoryIds)
    {
        Dictionary<MongoId, TemplateItem> templateItems = databaseService.GetItems();
        List<TemplateItem> result = [];
        foreach (MongoId itemTpl in 
                 from baseClass in categoryIds 
                 from itemTpl in itemHelper.GetItemTplsOfBaseType(baseClass.ToString()) 
                 where itemHelper.IsValidItem(itemTpl) 
                 select itemTpl)
        {
            if (templateItems.TryGetValue(itemTpl, out TemplateItem? templateItem))
            {
                result.Add(templateItem);
            }
        }
        return result;
    }
}