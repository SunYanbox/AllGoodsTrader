using SPTarkov.Server.Core.Models.Common;

namespace AllGoodsTrader.Configs;

/// <summary>
/// 物品分类 Parent ID 常量
/// </summary>
public static class ItemCategories
{
    // 武器与配件
    public static readonly List<MongoId> WeaponsAndAccessories =
    [
        BaseClasses.ASSAULT_CARBINE, // 突击卡宾枪
        BaseClasses.ASSAULT_RIFLE, // 突击步枪
        BaseClasses.PISTOL, // 手枪
        BaseClasses.SHOTGUN, // 霰弹枪
        BaseClasses.SMG, // 冲锋枪
        BaseClasses.SNIPER_RIFLE, // 狙击步枪
        BaseClasses.MARKSMAN_RIFLE, // 精确射手步枪
        BaseClasses.MACHINE_GUN, // 机枪
        BaseClasses.GRENADE_LAUNCHER, // 榴弹发射器
        BaseClasses.LAUNCHER, // 发射器
        BaseClasses.ROCKET_LAUNCHER, // 火箭发射器
        BaseClasses.REVOLVER, // 左轮手枪
        BaseClasses.SPECIAL_WEAPON, // 特殊武器
        BaseClasses.KNIFE, // 刀
        BaseClasses.THROW_WEAP, // 投掷武器
        BaseClasses.ROCKET, // 火箭弹

        BaseClasses.BARREL, // 枪管
        BaseClasses.COMPENSATOR, // 补偿器
        BaseClasses.FLASH_HIDER, // 消焰器
        BaseClasses.SILENCER, // 消音器
        BaseClasses.MUZZLE, // 枪口
        BaseClasses.MUZZLE_COMBO, // 枪口组合件
        BaseClasses.GASBLOCK, // 导气块
        BaseClasses.HANDGUARD, // 护木
        BaseClasses.RECEIVER, // 机匣
        BaseClasses.STOCK, // 枪托
        BaseClasses.PISTOL_GRIP, // 手枪式握把
        BaseClasses.SHAFT, // 枪机框

        BaseClasses.ASSAULT_SCOPE, // 突击瞄具
        BaseClasses.COLLIMATOR, // 准直瞄具
        BaseClasses.COMPACT_COLLIMATOR, // 紧凑型准直瞄具
        BaseClasses.IRON_SIGHT, // 机械瞄具
        BaseClasses.OPTIC_SCOPE, // 光学瞄准镜
        BaseClasses.SPECIAL_SCOPE, // 特殊瞄准镜
        BaseClasses.SIGHTS, // 瞄具

        BaseClasses.BIPOD, // 两脚架
        BaseClasses.FOREGRIP, // 前握把
        BaseClasses.FLASHLIGHT, // 手电筒
        BaseClasses.LIGHT_LASER, // 激光指示器
        BaseClasses.MOUNT, // 导轨座
        BaseClasses.RAIL_COVERS, // 导轨护盖
        BaseClasses.TACTICAL_COMBO, // 战术组合件

        BaseClasses.MAGAZINE, // 弹匣
        BaseClasses.CYLINDER_MAGAZINE, // 弹鼓
        BaseClasses.SPRING_DRIVEN_CYLINDER, // 弹簧驱动弹筒

        BaseClasses.AUXILIARY_MOD, // 辅助改装件
        BaseClasses.FUNCTIONAL_MOD, // 功能改装件
        BaseClasses.GEAR_MOD, // 装备改装件
        BaseClasses.MASTER_MOD, // 主改装件
        BaseClasses.MOD // 改装件
        // BaseClasses.BUILT_IN_INSERTS // 内置插板
    ];

    // 食物饮品与药品
    public static readonly List<MongoId> FoodDrinkAndMedical =
    [
        BaseClasses.DRINK, // 饮品
        BaseClasses.FOOD, // 食物
        BaseClasses.FOOD_DRINK, // 食品饮料

        BaseClasses.DRUGS, // 药品
        BaseClasses.MEDICAL, // 医疗物品
        BaseClasses.MEDICAL_SUPPLIES, // 医疗用品
        BaseClasses.MEDS, // 医疗物资
        BaseClasses.MED_KIT, // 医疗包
        BaseClasses.STIMULATOR
    ];

    // 装备与弹药
    public static readonly List<MongoId> EquipmentAndAmmo =
    [
        BaseClasses.ARMOR, // 护甲
        BaseClasses.ARMOR_PLATE, // 装甲板
        BaseClasses.ARMORED_EQUIPMENT, // 装甲装备
        BaseClasses.VEST, // 胸挂/弹挂
        BaseClasses.HEADWEAR, // 头戴装备
        BaseClasses.HEADPHONES, // 耳机
        BaseClasses.FACE_COVER, // 面部防护
        BaseClasses.VISORS, // 面罩
        BaseClasses.BACKPACK, // 背包
        BaseClasses.EQUIPMENT, // 装备

        BaseClasses.AMMO, // 弹药
        BaseClasses.AMMO_BOX, // 弹药箱

        BaseClasses.ARM_BAND, // 臂章
        BaseClasses.NIGHT_VISION, // 夜视仪
        BaseClasses.THERMAL_VISION, // 热成像仪
        BaseClasses.COMPASS, // 指南针
        BaseClasses.PORTABLE_RANGE_FINDER
    ];

    // 杂物与其他
    public static readonly List<MongoId> Miscellaneous =
    [
        BaseClasses.BARTER_ITEM, // 交易物品
        BaseClasses.BATTERY, // 电池
        BaseClasses.BUILDING_MATERIAL, // 建筑材料
        BaseClasses.CHARGE, // 充电器
        BaseClasses.COMPOUND_ITEM, // 复合物品
        BaseClasses.CULTIST_AMULET, // 邪教护身符
        BaseClasses.ELECTRONICS, // 电子设备
        BaseClasses.FUEL, // 燃料
        BaseClasses.FLYER, // 传单
        BaseClasses.HOUSEHOLD_GOODS, // 家居用品
        BaseClasses.INFO, // 信息物品
        BaseClasses.INVENTORY, // 库存物品
        BaseClasses.ITEM, // 物品
        BaseClasses.JEWELRY, // 珠宝
        BaseClasses.KEY, // 钥匙
        BaseClasses.KEY_MECHANICAL, // 机械钥匙
        BaseClasses.KEYCARD, // 钥匙卡
        BaseClasses.LOCKABLE_CONTAINER, // 可上锁容器
        BaseClasses.LOOT_CONTAINER, // 战利品容器
        BaseClasses.LUBRICANT, // 润滑剂
        BaseClasses.MAP, // 地图
        BaseClasses.MARK_OF_UNKNOWN, // 未知标记
        BaseClasses.MOB_CONTAINER, // 动态容器
        BaseClasses.MONEY, // 货币
        BaseClasses.MULTITOOLS, // 多功能工具
        BaseClasses.OTHER, // 其他
        BaseClasses.PLANTING_KITS, // 种植工具包
        BaseClasses.PMS, // 个人医疗站
        BaseClasses.POCKETS, // 口袋
        BaseClasses.RADIO_TRANSMITTER, // 无线电发射器
        BaseClasses.RANDOM_LOOT_CONTAINER, // 随机战利品容器
        BaseClasses.REPAIR_KITS, // 维修工具包
        BaseClasses.SEARCHABLE_ITEM, // 可搜索物品
        BaseClasses.SIMPLE_CONTAINER, // 简单容器
        BaseClasses.SORTING_TABLE, // 整理台
        BaseClasses.SPEC_ITEM, // 特殊物品
        BaseClasses.STACKABLE_ITEM, // 可堆叠物品
        BaseClasses.STASH, // 藏匿处
        BaseClasses.STATIONARY_CONTAINER, // 固定容器
        BaseClasses.TOOL, // 工具
        BaseClasses.HIDEOUT_AREA_CONTAINER
    ];
}
