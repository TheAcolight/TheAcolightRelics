using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using TheAcolightRelics.TheAcolightRelicsCode.Relics;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics;

public class EclipseInAJar() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        EclipseInAJar eclipseInAJar = this;
        if (player != eclipseInAJar.Owner || eclipseInAJar.Owner.PlayerCombatState.TurnNumber > 1)
            return;

        foreach (CardModel card2 in (IEnumerable<CardModel>)PileType.Draw.GetPile(eclipseInAJar.Owner).Cards
                     .Where<CardModel>(new Func<CardModel, bool>(eclipseInAJar.Filter)).ToList<CardModel>())
        {
            CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card2, PileType.Hand);
        }
        eclipseInAJar.Flash();
    }
    
    private bool Filter(CardModel card)
    {
        bool isCurse = card.Type == CardType.Curse;
        return isCurse;
    }
}