using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TheAcolightRelics.TheAcolightRelicsCode.Relics;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics.Dark1;

public class EnchantingVeil() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    public override async Task AfterAutoPostPlayPhaseEntered(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        EnchantingVeil enchantingVeil = this;
        CardPile drawPile;
        CardPile discardPile;
        if (player != enchantingVeil.Owner)
        {
            drawPile = (CardPile) null;
            discardPile = (CardPile) null;
        }
        else
        {
            drawPile = PileType.Draw.GetPile(enchantingVeil.Owner);
            discardPile = PileType.Discard.GetPile(enchantingVeil.Owner);
            

            List<CardModel> list1 = drawPile.Cards.Where<CardModel>((Func<CardModel, bool>) (c => c.Type == CardType.Power && !c.Keywords.Contains(CardKeyword.Unplayable))).ToList<CardModel>();
            List<CardModel> list2 = discardPile.Cards.Where<CardModel>((Func<CardModel, bool>) (c => c.Type == CardType.Power && !c.Keywords.Contains(CardKeyword.Unplayable))).ToList<CardModel>();
            List<CardModel> list3 = list1.Concat(list2).ToList<CardModel>();
            // With a cost equal to or lower than your energy
            List<CardModel> list4 = list3.Where<CardModel>((Func<CardModel, bool>) (card => card.EnergyCost.GetAmountToSpend() <= player.PlayerCombatState.Energy)).ToList<CardModel>();
            CardModel card = enchantingVeil.Owner.RunState.Rng.Shuffle.NextItem<CardModel>((IEnumerable<CardModel>) list4);
            if (card != null)
                await CardCmd.AutoPlay(choiceContext, card, (Creature) null);
            
            drawPile = (CardPile) null;
            discardPile = (CardPile) null;
        }
    }
}