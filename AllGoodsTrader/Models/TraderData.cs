using JetBrains.Annotations;
using SPTarkov.Server.Core.Models.Common;

namespace AllGoodsTrader.Models;

/// <summary>
/// 商人数据实体类
/// </summary>
public class TraderData
{
    /// <summary>
    /// 商人唯一标识符（MongoDB ObjectId）
    /// </summary>
    public MongoId Id { get; init; }

    /// <summary>
    /// 商人的国际化数据
    /// </summary>
    public Dictionary<string, TraderLocales> Locales { get; init; } = new();
    
    /// <summary>
    /// 商人的默认国际化数据
    /// </summary>
    public const string DefaultLocale = "en";
    
    /// <summary>
    /// 商人头像文件名
    /// </summary>
    public required string Avatar { get; init; }

    /// <summary>
    /// 商人头像文件路径
    /// </summary>
    [UsedImplicitly]
    public string AvatarFilePath => ImgPath + Avatar;

    /// <summary>
    /// 是否启用维修功能
    /// </summary>
    public bool AvailableRepair { get; set; }
    
    /// <summary>
    /// 商人对应的物品类型
    /// </summary>
    public required List<MongoId> BaseClasses { get; set; }
    
    /// <summary>
    /// 是否启用保险功能
    /// </summary>
    public bool AvailableInsurance { get; set; }
    
    private const string ImgPath = "data/res/";
}