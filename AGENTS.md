# AllGoodsTrader 项目上下文

## 项目概述

**AllGoodsTrader** 是一个为《逃离塔科夫》SPT (Single Player Tarkov) 服务器开发的模组（Mod），版本 `0.5.0`，支持 SPT `4.0.8`。

该模组添加了一个完整的商品商人系统，包含四种类型的商人：

| 商人类型 | 标识名 | 功能 |
|---------|--------|------|
| 武器配件商 | `WeaponsTrader` | 交易各类武器及改装配件 |
| 装备商 | `GearTrader` | 交易护甲、头盔、背包等装备，**支持维修和保险** |
| 消耗品商 | `ConsumablesTrader` | 交易食物、饮品、医疗物品 |
| 杂物商 | `MiscTrader` | 交易杂物、钥匙等，**可购买所有安全箱容器** |

### 核心特性

- **双语支持**: 中文 (ch) 和英文 (en) 的名称、描述
- **特殊物品处理**:
  - 火箭筒自动配弹并计算总价
  - 弹药盒自动填充弹药并设置堆叠数量
  - 有槽位的物品默认随机填充兼容物品
- **统计追踪**: 内置安全箱、特殊物品、有槽位物品、普通物品的添加成功统计和调试日志
- **定价系统**: 支持三种模式，可配置全局价格倍率

## 技术栈

- **语言**: C# (.NET 9.0)
- **框架**: SPTarkov Server Core `4.0.8`
- **依赖注入**: SPTarkov.DI
- **构建工具**: MSBuild (自定义 Target 实现自动化部署)

## 项目结构

```
AllGoodsTrader/
├── AllGoodsTrader.sln          # 解决方案文件
├── AllGoodsTrader/
│   ├── AllGoodsTrader.csproj   # 项目文件（含自动化构建/部署脚本）
│   ├── ModMetadata.cs          # 模组元数据（名称、版本、作者等）
│   ├── Configs/
│   │   ├── ItemCategories.cs   # 物品分类 Parent ID 常量定义
│   │   ├── ModConfig.cs        # 模组配置（价格倍率、价格模式）
│   │   └── PriceModeEnum.cs    # 价格模式枚举常量
│   ├── Models/
│   │   ├── TraderConfigs.cs    # 商人配置（4个商人的完整数据）
│   │   ├── TraderData.cs       # 商人数据实体类
│   │   ├── TraderFactory.cs    # 商人注册和物品添加核心逻辑
│   │   └── TraderLocales.cs    # 商人本地化数据
│   ├── Services/
│   │   └── ItemCategoryService.cs  # 物品分类服务（价格计算、物品筛选）
│   ├── SptHelpers/
│   │   ├── AddCustomTraderHelper.cs    # 自定义商人添加辅助类
│   │   └── FluentTraderAssortCreator.cs # 流式商人商品清单创建器
│   └── data/                   # 静态资源目录（图片、配置等）
└── README.md
```

## 构建和运行

### 构建命令

```bash
# Release 构建
dotnet build -c Release

# 构建并自动打包为 7z（需要在 .csproj.user 中配置 SPTPath）
dotnet build -c Release
```

### 部署

项目配置了自动化部署：

1. **构建后自动拷贝**: 设置 `SPTPath` 环境变量或在 `.csproj.user` 中配置，构建后会自动将 DLL 和 data 文件夹复制到 SPT 模组目录
2. **自动打包**: 构建后会使用 7-Zip 将 SPT 文件夹压缩为 `AllGoodsTrader-{Version}.7z`

### 配置文件

模组首次运行会自动生成配置文件 `data/config.json`

## 开发约定

### 代码风格

- 使用 C# 12+ 特性（record、primary constructor 等）
- 使用 `Nullable` 和 `ImplicitUsings`
- 依赖注入通过 `[Injectable]` 特性声明，支持优先级排序（`TypePriority`）
- 日志使用 `ISptLogger<T>` 进行

### 架构模式

- **依赖注入**: 所有核心服务通过构造函数注入
- **生命周期管理**: 实现 `IOnLoad` 接口在游戏加载时执行初始化
- **流式 API**: `FluentTraderAssortCreator` 采用链式调用模式创建商品清单
- **配置驱动**: 商人配置与业务逻辑分离

### 物品处理逻辑

`TraderFactory.HandleSpecialItems()` 处理特殊物品：
1. 火箭筒 (`RShG-2`) → 自动配弹 `725mm` 火箭弹
2. 弹药盒 → 遍历 `StackSlots` 填充所有兼容弹药

### 价格计算

`ItemCategoryService.GetItemPrice()` 根据配置模式计算：
- `Handbook`: 使用手册价格
- `AvgRagfair`: 使用跳蚤市场平均价
- `Auto`: 取两者最小值
- 最终结果乘以 `priceModify` 倍率

## 关键文件说明

| 文件 | 说明 |
|------|------|
| `TraderFactory.cs` | 核心逻辑：商人注册、物品添加、特殊物品处理、本地化 |
| `ItemCategoryService.cs` | 物品筛选和价格计算服务 |
| `TraderConfigs.cs` | 4个商人的完整配置（ID、语言、分类、头像等） |
| `ItemCategories.cs` | 物品分类常量定义，按 Parent ID 分组 |
| `FluentTraderAssortCreator.cs` | 流式 API 创建商人商品清单 |
