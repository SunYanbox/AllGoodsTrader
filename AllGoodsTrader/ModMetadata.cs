using SPTarkov.Server.Core.Models.Spt.Mod;

namespace AllGoodsTrader;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.suntion.allgoodstrader";
    public string Name { get; init; } = "AllGoodsTrader";
    public string Author { get; init; } = "Suntion";
    public List<string>? Contributors { get; init; } = [];
    public SemanticVersioning.Version Version { get; init; } = new("2.0.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.3");


    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; } = "https://github.com/SunYanbox/AllGoodsTrader";
    public bool? IsBundleMod { get; init; } = false;
    public string License { get; init; } = "CC-BY-SA";
    /// <summary>
    /// Indicates whether the mod uses Prepatcher.
    /// Set to true if the mod contains Prepatcher patches; otherwise leave false.
    /// </summary>
    public bool HasPrepatcher { get; init; } = false;
}
