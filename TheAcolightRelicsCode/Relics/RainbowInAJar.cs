using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheAcolightRelics.TheAcolightRelicsCode.Relics;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics;


public class RainbowInAJar() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        RainbowInAJar rainbowInAJar = this;
        if (player != rainbowInAJar.Owner || rainbowInAJar.Owner.PlayerCombatState.TurnNumber > 1)
            return;
        List<CardModel> list = PileType.Draw.GetPile(player).Cards.ToList<CardModel>();
        if (list.Count == 0)
            return;
        CardModel card = player.RunState.Rng.CombatCardSelection.NextItem<CardModel>((IEnumerable<CardModel>) list);
        rainbowInAJar.Flash();
        CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card, PileType.Hand);
    }
}