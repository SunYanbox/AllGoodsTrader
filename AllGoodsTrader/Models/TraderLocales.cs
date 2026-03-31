namespace AllGoodsTrader.Models;

public class TraderLocales
{
    /// <summary>
    /// 商人正式名称
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 商人昵称/别名
    /// </summary>
    public required string Nickname { get; set; }

    /// <summary>
    /// 商人所在位置/据点
    /// </summary>
    public required string Location { get; set; }

    /// <summary>
    /// 商人详细描述信息
    /// </summary>
    public required string Description { get; set; }
}