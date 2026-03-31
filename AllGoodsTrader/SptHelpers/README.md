此处是完全来自于[sp-tarkov/server-mod-examples](https://github.com/sp-tarkov/server-mod-examples)的代码

- [FluentTraderAssortCreator](https://github.com/sp-tarkov/server-mod-examples/blob/main/13.1AddTraderWithDynamicAssorts/FluentTraderAssortCreator.cs)

- [AddCustomTraderHelper](https://github.com/sp-tarkov/server-mod-examples/blob/main/13AddTraderWithAssortJson/AddCustomTraderHelper.cs)

本模组内除了命名空间外不对这两个文件中的代码做任何修改，方便更新(如果需要)

忽略的原文件的警告:

```csharp
// ReSharper disable GrammarMistakeInComment
// ReSharper disable UnusedVariable
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8601 // 引用类型赋值可能为 null。
#pragma warning disable CS9113 // 参数未读。
```

创建复杂物品的参考代码:

```csharp
/// <summary>
/// Create a complete weapon from scratch.
/// Weapons start with a 'root' item
/// They there have various 'child' items that attach off of the root, the discord mod support can help direct you on how to figure our what you need
/// </summary>
/// <returns>A complete glock</returns>
public List<Item> CreateGlock()
{
    // Create an array ready to hold the glock and all its mods
    var glock = new List<Item>();

    // Add the base (root) first
    glock.Add(new Item
    { // Add the base weapon first
        Id =
        NewItemIds.GLOCK_BASE, // Ids matter, Ids MUST be unique for every item
        Template = new MongoId("5a7ae0c351dfba0017554310")
        , // This is the weapons tpl, found on: https://db.sp-tarkov.com/search
    });

    // Add barrel
    glock.Add(new Item
    {
        Id =
        NewItemIds.GLOCK_BARREL,
        Template = new MongoId("5a6b60158dc32e000a31138b"),
        ParentId =
        NewItemIds.GLOCK_BASE, // This is a sub item, you need to define its parent it is attached to / inserted into
        SlotId =
        "mod_barrel", // Required for mods, you need to define what 'slot' the mod will fill on the weapon
    });

    // Add receiver
    glock.Add(new Item
    {
        Id =
        NewItemIds.GLOCK_RECIEVER,
        Template = new MongoId("5a9685b1a2750c0032157104"),
        ParentId =
        NewItemIds.GLOCK_BASE,
        SlotId =
        "mod_reciever",
    });

    // Add compensator
    glock.Add(new Item
    {
        Id =
        NewItemIds.GLOCK_COMPENSATOR,
        Template =
        new MongoId("5a7b32a2e899ef00135e345a"),
        ParentId =
        NewItemIds.GLOCK_RECIEVER, // The parent of this mod is the receiver NOT weapon, be careful to get the correct parent
        SlotId =
        "mod_muzzle",
    });

    // Add Pistol grip
    glock.Add(new Item
    {
        Id =
        NewItemIds.GLOCK_PISTOL_GRIP,
        Template =
        new MongoId("5a7b4960e899ef197b331a2d"),
        ParentId =
        NewItemIds.GLOCK_BASE,
        SlotId =
        "mod_pistol_grip",
    });

    // Add front sight
    glock.Add(new Item
    {
        Id =
        NewItemIds.GLOCK_FRONT_SIGHT,
        Template =
        new MongoId("5a6f5d528dc32e00094b97d9"),
        ParentId =
        NewItemIds.GLOCK_RECIEVER,
        SlotId =
        "mod_sight_rear",
    });

    // Add rear sight
    glock.Add(new Item
    {
        Id =
        NewItemIds.GLOCK_REAR_SIGHT,
        Template =
        new MongoId("5a6f58f68dc32e000a311390"),
        ParentId =
        NewItemIds.GLOCK_RECIEVER,
        SlotId =
        "mod_sight_front",
    });

    // Add magazine
    glock.Add(new Item
    {
        Id =
        NewItemIds.GLOCK_MAGAZINE,
        Template =
        new MongoId("630769c4962d0247b029dc60"),
        ParentId =
        NewItemIds.GLOCK_BASE,
        SlotId =
        "mod_magazine",
    });

    return glock;
}
```