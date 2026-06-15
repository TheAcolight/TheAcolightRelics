using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheAcolightRelics.TheAcolightRelicsCode.Relics;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics;

public class LightBulb() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(0)
    ];
    
    public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        LightBulb source = this;
        int currentEnergy = player.PlayerCombatState.Energy;
        if (player != source.Owner.Creature.Player || !Hook.ShouldFlush(player.Creature.CombatState, player))
            return;
        if (currentEnergy <= 0)
            return;
        source.DynamicVars.Cards.BaseValue = currentEnergy;
        CardSelectorPrefs prefs = new CardSelectorPrefs(source.SelectionScreenPrompt, 0, currentEnergy);
        List<CardModel> list = (await CardSelectCmd.FromHand(choiceContext, player, prefs, new Func<CardModel, bool>(source.RetainFilter), (AbstractModel) source)).ToList<CardModel>();
        if (list.Count == 0)
            return;
        foreach (CardModel cardModel in list)
            cardModel.GiveSingleTurnRetain();
    }

    private bool RetainFilter(CardModel card) => !card.ShouldRetainThisTurn;
    
}