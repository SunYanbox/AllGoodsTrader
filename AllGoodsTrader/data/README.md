# Merchant Data File Overview / 商人数据文件概述

`traders/{merchant_id}.jsonc` (参考 `data.jsonc`)

## Default Config / 默认配置

```jsonc
{
  // Is this item enabled? / 是否启用此物品
  "enabled": true,
  
  // Is repair function enabled? / 是否启用维修功能
  "availableRepair": false,
  
  // Is insurance function enabled? / 是否启用保险功能
  "availableInsurance": false,
  
  // Overridable properties (Incorrect rewriting may cause the mod to overwrite all data with default values; backup before each modification): / 可重写属性 (重写的数据有误会导致模组使用默认值覆盖所有数据，每次修改前建议备份):
  
  // Default settings for 'insurance' are used only if not manually set / 没有手动设置 `insurance` 时才会使用 `availableInsurance` 对应的默认设置
  "insurance": {
    "availability": false,
    "excluded_category": [],
    "max_return_hour": 0,
    "max_storage_time": 99,
    "min_payment": 0,
    "min_return_hour": 0
  },
  
  // Default settings for 'repair' are used only if not manually set / 没有手动设置 `repair` 时才会使用 `availableRepair` 对应的默认设置
  "repair": {
    "availability": false,
    "currency": "5449016a4bdc2d6f028b456f",
    "currency_coefficient": 1,
    "excluded_category": [
      "5447e1d04bdc2dff2f8b4567"
    ],
    "excluded_id_list": [],
    "quality": "0"
  }
  // Other properties omitted / 其他属性省略
}
```

## WeaponsTrader / 武器配件商人

`traders/68dcbdecd6e04c263b42f6ba.json`

## GearTrader / 装备商人配置

`traders/68e24cdc607f5c9ae44c27b1.json`

Default Config / 默认配置:

- `availableRepair`: true
- `availableInsurance`: true

## ConsumablesTrader / 消耗品商人配置

`traders/68e24cd2607f5c9ae44c27b0.json`

## MiscTrader / 杂物商人配置

`traders/68e24cdc607f5c9ae44c27b2.json`
