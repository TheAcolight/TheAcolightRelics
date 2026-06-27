using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;
using TheAcolightRelics.TheAcolightRelicsCode.Enchantments;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics.Dark1;

public class DreamIchor() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Amalgamate",2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromEnchantment<Amalgamate>(DynamicVars["Amalgamate"].IntValue).First()
    ];
    
    public override async Task AfterObtained()
    {
        DreamIchor dreamIchor = this;
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        foreach (CardModel card in await CardSelectCmd.FromDeckForEnchantment(dreamIchor.Owner, (EnchantmentModel) ModelDb.Enchantment<Amalgamate>(), DynamicVars["Amalgamate"].IntValue, prefs))
        {
            CardCmd.Enchant<Amalgamate>(card, DynamicVars["Amalgamate"].IntValue);
            CardCmd.Preview(card);
        }
    }

}