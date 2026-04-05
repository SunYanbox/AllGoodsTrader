using System.Text.Json.Serialization;
using AllGoodsTrader.Models;
using JetBrains.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;

namespace AllGoodsTrader.Configs;

public sealed record ModTraderConfig : TraderBase
{
    [JsonPropertyName("enabled")] public bool Enabled { get; set; } = true;

    /// <summary>
    /// 是否启用维修功能
    /// </summary>
    [JsonPropertyName("availableRepair")]
    public bool? AvailableRepair { get; set; }

    /// <summary>
    /// 是否启用保险功能
    /// </summary>
    [JsonPropertyName("availableInsurance")]
    public bool? AvailableInsurance { get; set; }

    [JsonPropertyName("isAvailableInPVE")] public new bool? IsAvailableInPVE { get; set; }

    [JsonPropertyName("isCanTransferItems")]
    public new bool? IsCanTransferItems { get; set; }

    [JsonPropertyName("isCanTransferItemsFromPve")]
    public new bool? IsCanTransferItemsFromPve { get; set; }

    #region 不会在自动初始化时写入文件

    [UsedImplicitly]
    [JsonIgnore]
    [JsonPropertyName("_id")]
    public override MongoId Id { get; set; }

    [UsedImplicitly]
    [JsonIgnore]
    [JsonPropertyName("avatar")]
    public override string? Avatar { get; set; }

    [UsedImplicitly]
    [JsonIgnore]
    [JsonPropertyName("location")]
    public override string? Location { get; set; }

    [UsedImplicitly]
    [JsonIgnore]
    [JsonPropertyName("name")]
    public override string Name { get; set; } = string.Empty;

    [UsedImplicitly]
    [JsonIgnore]
    [JsonPropertyName("nickname")]
    public override string? Nickname { get; set; }

    [UsedImplicitly]
    [JsonIgnore]
    [JsonPropertyName("surname")]
    public new string? Surname { get; set; }

    #endregion

    /// <summary>
    /// 将模组商人配置转换为SPT商人配置
    /// </summary>
    public TraderBase AsTraderBase(TraderData traderData)
    {
        return new TraderBase
        {
            #region 自动赋值属性(无法被重写)

            Id = traderData.Id,
            Avatar = $"/files/trader/avatar/{traderData.Avatar}",
            Location = traderData.Locales[TraderData.DefaultLocale].Location,
            Name = traderData.Locales[TraderData.DefaultLocale].Name,
            Nickname = traderData.Locales[TraderData.DefaultLocale].Nickname,
            Surname = traderData.Locales[TraderData.DefaultLocale].Nickname,

            #endregion


            #region 有默认值的可选属性

            RefreshTraderRagfairOffers = RefreshTraderRagfairOffers,
            AvailableInRaid = AvailableInRaid ?? false,
            BalanceDollar = BalanceDollar ?? 0,
            BalanceEuro = BalanceEuro ?? 0,
            BalanceRub = BalanceRub ?? 7000_0000,
            BuyerUp = BuyerUp ?? false,
            Currency = Currency ?? CurrencyType.RUB,
            CustomizationSeller = CustomizationSeller ?? false,
            Discount = Discount ?? 0,
            DiscountEnd = DiscountEnd ?? 0,
            GridHeight = GridHeight ?? 120,
            ProhibitedItemsSellModifier = ProhibitedItemsSellModifier ?? 0,
            Insurance = Insurance ?? new TraderInsurance
            {
                Availability = AvailableInsurance ?? traderData.AvailableInsurance,
                ExcludedCategory = [],
                MaxReturnHour = 0,
                MaxStorageTime = 99,
                MinPayment = 0,
                MinReturnHour = 0
            },
            ItemsBuy = ItemsBuy ?? new ItemBuyData
            {
                Category =
                [
                    BaseClasses.ITEM
                ],
                IdList = []
            },
            ItemsBuyProhibited = ItemsBuyProhibited ?? new ItemBuyData
            {
                Category = [],
                IdList = []
            },
            ItemsSell = ItemsSell ?? null,
            IsAvailableInPVE = IsAvailableInPVE ?? true,
            IsCanTransferItems = IsCanTransferItems ?? false,
            IsCanTransferItemsFromPve = IsCanTransferItemsFromPve ?? false,
            TransferableItems = TransferableItems ?? new ItemBuyData
            {
                Category = [],
                IdList = []
            },
            ProhibitedTransferableItems = ProhibitedTransferableItems ?? new ItemBuyData
            {
                Category = [],
                IdList = []
            },
            LoyaltyLevels = LoyaltyLevels ??
            [
                new TraderLoyaltyLevel
                {
                    BuyPriceCoefficient = 30,
                    ExchangePriceCoefficient = 0,
                    HealPriceCoefficient = 0,
                    InsurancePriceCoefficient = 20,
                    MinLevel = 1,
                    MinSalesSum = 0,
                    MinStanding = 0,
                    RepairPriceCoefficient = 130
                }
            ],
            MainDialogue = MainDialogue,
            Medic = Medic ?? false,
            NextResupply = NextResupply ?? 0,
            Repair = Repair ?? new TraderRepair
            {
                Availability = AvailableRepair ?? traderData.AvailableRepair,
                Currency = Money.ROUBLES,
                CurrencyCoefficient = 1,
                ExcludedCategory = [],
                ExcludedIdList = [],
                Quality = 0,
                PriceRate = 0.8
            },
            SellCategory = SellCategory ?? [],
            UnlockedByDefault = UnlockedByDefault ?? true,
            ExtensionData = ExtensionData,

            #endregion
        };
    }
}