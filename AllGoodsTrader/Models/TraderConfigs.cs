using AllGoodsTrader.Configs;
using SPTarkov.Server.Core.Models.Common;

namespace AllGoodsTrader.Models;

/// <summary>
/// 商人配置
/// </summary>
public static class TraderConfigs
{
    public static TraderData[] AllTraderConfigs =>
    [
        WeaponsTrader,
        GearTrader,
        ConsumablesTrader,
        MiscTrader
    ];
    
    /// <summary>
    /// 武器配件商人配置
    /// </summary>
    public static readonly TraderData WeaponsTrader = new()
    {
        Id = new MongoId("68dcbdecd6e04c263b42f6ba"),
        Locales = new Dictionary<string, TraderLocales>
        {
            {
                "ch", new TraderLocales
                {
                    Name = "武器配件商",
                    Nickname = "枪匠",
                    Location = "武器工坊",
                    Description = "专注于枪械与配件的交易商，提供各类武器及改装配件"
                }
            },
            {
                "en", new TraderLocales
                {
                    Name = "Weapons & Mods Trader",
                    Nickname = "Gunsmith",
                    Location = "Weapon Workshop",
                    Description = "Specialized in firearms and modifications, offering various weapons and upgrade parts"
                }
            }
        },
        Avatar = "gun_mod_trader.png",
        BaseClasses = ItemCategories.WeaponsAndAccessories
    };

    /// <summary>
    /// 装备商人配置
    /// </summary>
    public static readonly TraderData GearTrader = new()
    {
        Id = new MongoId("68e24cdc607f5c9ae44c27b1"),
        Locales = new Dictionary<string, TraderLocales>
        {
            {
                "ch", new TraderLocales
                {
                    Name = "装备商",
                    Nickname = "护甲师",
                    Location = "装备仓库",
                    Description = "提供护甲、头盔、背包、胸挂等战术装备"
                }
            },
            {
                "en", new TraderLocales
                {
                    Name = "Gear Trader",
                    Nickname = "Armorer",
                    Location = "Equipment Warehouse",
                    Description = "Provides tactical equipment including armor, helmets, backpacks, and chest rigs"
                }
            }
        },
        AvailableRepair = true,
        AvailableInsurance = true,
        Avatar = "equipment_trader.png",
        BaseClasses = ItemCategories.EquipmentAndAmmo
    };

    /// <summary>
    /// 消耗品商人配置
    /// </summary>
    public static readonly TraderData ConsumablesTrader = new()
    {
        Id = new MongoId("68e24cd2607f5c9ae44c27b0"),
        Locales = new Dictionary<string, TraderLocales>
        {
            {
                "ch", new TraderLocales
                {
                    Name = "消耗品商",
                    Nickname = "医师",
                    Location = "医疗站",
                    Description = "供应食物、饮品、医疗物品及各类消耗品"
                }
            },
            {
                "en", new TraderLocales
                {
                    Name = "Consumables Trader",
                    Nickname = "Medic",
                    Location = "Medical Station",
                    Description = "Supplies food, drinks, medical items, and various consumables"
                }
            }
        },
        Avatar = "consumables_trader.png",
        BaseClasses = ItemCategories.FoodDrinkAndMedical
    };

    /// <summary>
    /// 杂物商人配置
    /// </summary>
    public static readonly TraderData MiscTrader = new()
    {
        Id = new MongoId("68e24cdc607f5c9ae44c27b2"),
        Locales = new Dictionary<string, TraderLocales>
        {
            {
                "ch", new TraderLocales
                {
                    Name = "杂物商",
                    Nickname = "收藏家",
                    Location = "旧货市场",
                    Description = "收购各类杂物、以物易物物品、钥匙等",
                }
            },
            {
                "en", new TraderLocales
                {
                    Name = "Miscellaneous Trader",
                    Nickname = "Collector",
                    Location = "Flea Market",
                    Description = "Buys various barter items, keys, and miscellaneous goods"
                }
            }
        },
        Avatar = "goods_trader.png",
        BaseClasses = ItemCategories.Miscellaneous
    };
}
