using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using TheAcolightRelics.TheAcolightRelicsCode.Relics;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics.Dark1;

public class NightFeather() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;

    private const int _swiftAmount = 1;
    
    public override bool HasUponPickupEffect => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromEnchantment<Swift>(_swiftAmount).First()
    ];

    public override async Task AfterObtained()
    {
        NightFeather nightFeather = this;
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 0, nightFeather.DynamicVars.Cards.IntValue)
        {
            Cancelable = false,
            RequireManualConfirmation = true
        };
        Swift canonicalEnchantment = ModelDb.Enchantment<Swift>();
        foreach (CardModel card in await CardSelectCmd.FromDeckForEnchantment(nightFeather.Owner, (EnchantmentModel) canonicalEnchantment, 1, prefs))
        {
            CardCmd.Enchant(canonicalEnchantment.ToMutable(), card, card.EnergyCost.Canonical);
            CardCmd.Preview(card);
        }
        canonicalEnchantment = (Swift) null;
    }
}