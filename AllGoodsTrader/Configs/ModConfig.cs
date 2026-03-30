using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace AllGoodsTrader.Configs;

internal sealed record ModConfig
{
    /// <summary> 价格修正 </summary>
    [JsonPropertyName("priceModify")]
    [UsedImplicitly]
    public double? PriceModify { get; set; } = 1.2d;
    /// <summary>
    /// 基础价格计算方式: <br />
    /// - Handbook: 仅手册价格 <br />
    /// - AvgRagfair: 平均跳蚤价格 <br />
    /// - Auto: Min(手册价格, 平均跳蚤价格)
    /// </summary>
    [JsonPropertyName("priceMode")]
    [UsedImplicitly]
    public string? PriceMode { get => _priceMode ?? PriceModeEnum.Auto;
        set
        {
            _priceMode = value switch
            {
                PriceModeEnum.Handbook => PriceModeEnum.Handbook,
                PriceModeEnum.AvgRagfair => PriceModeEnum.AvgRagfair,
                _ => PriceModeEnum.Auto
            };
        } }
    
    
    [JsonIgnore] private string? _priceMode;
}