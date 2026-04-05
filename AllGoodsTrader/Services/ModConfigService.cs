using System.Reflection;
using AllGoodsTrader.Configs;
using AllGoodsTrader.Models;
using JetBrains.Annotations;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;

namespace AllGoodsTrader.Services;

[Injectable(InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public sealed class ModConfigService(
    JsonUtil jsonUtil,
    ModHelper modHelper,
    ISptLogger<ModConfigService> logger) : IOnLoad
{
    public ModConfig Config { get; private set; } = new();

    /// <summary>
    /// 商人配置信息
    /// </summary>
    [UsedImplicitly]
    public Dictionary<MongoId, ModTraderConfig> Traders { get; private set; } = new();

    private string PathToMod => modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

    private string ConfigPath => Path.Combine(PathToMod, "data", "config.json");
    private string TradersPath => Path.Combine(PathToMod, "data", "traders");

    public Task OnLoad()
    {
        Directory.CreateDirectory(TradersPath);

        Config = Load<ModConfig>(ConfigPath);

        foreach (MongoId traderId in TraderConfigs.ExistTraderIds)
        {
            var traderPath = $"{traderId.ToString()}.json";
            string traderConfigPath = Path.Combine(TradersPath, traderPath);
            var traderConfig = Load<ModTraderConfig>(traderConfigPath);
            Traders[traderId] = traderConfig;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 从指定路径加载 JSON 配置文件并反序列化为指定类型的实例
    /// </summary>
    /// <typeparam name="T">要加载的配置类型，必须具有无参构造函数</typeparam>
    /// <param name="path">配置文件路径</param>
    /// <returns>
    /// 反序列化得到的配置实例。
    /// 如果加载失败或文件不存在，则返回默认实例并创建新的配置文件。
    /// </returns>
    /// <remarks>
    /// 加载流程：
    /// 1. 尝试从指定路径读取并反序列化 JSON 文件
    /// 2. 如果成功，返回反序列化的实例
    /// 3. 如果失败（文件不存在、格式错误或其他异常），记录错误日志
    /// 4. 创建 T 类型的默认实例
    /// 5. 将默认实例序列化为 JSON 并写入配置文件（用于生成默认配置模板）
    /// 6. 返回默认实例
    /// </remarks>
    private T Load<T>(string path) where T : new()
    {
        T? result = default;
        try
        {
            result = jsonUtil.DeserializeFromFile<T>(path);
        }
        catch (Exception e)
        {
            logger.Error($"加载配置文件\"{path}\"失败, 将使用默认配置", e);
        }

        if (result is not null) return result;

        result = new T();
        File.WriteAllText(path, jsonUtil.Serialize(result, true));

        return result;
    }
}