using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using TheAcolightRelics.TheAcolightRelicsCode.Fields;

namespace TheAcolightRelics.TheAcolightRelicsCode.Enchantments;

public class Amalgamate : TheAcolightRelicsEnchantment
{
    public override bool ShowAmount => true;

    private void ResetTracker()
    {
        CardModel targetCard = this.Card.DeckVersion ?? this.Card;
        AmalgamateFields.CombatsSinceLastRestOrEnchanted.Set(targetCard, 0);
    }

    protected override void OnEnchant()
    {
        CardModel targetCard = this.Card.DeckVersion ?? this.Card;
        if (AmalgamateFields.AmalgamateNewEnchantment.Get(targetCard)) ResetTracker();
        AmalgamateFields.AmalgamateNewEnchantment.Set(targetCard, false);
    }

    public override Task AfterRestSiteHeal(Player player, bool isMimicked)
    {
        ResetTracker();
        return Task.CompletedTask;
    }

    protected int CombatsLeft
    {
        get
        {            
            CardModel targetCard = this.Card.DeckVersion ?? this.Card;
            int combatsLeft = this.Amount - AmalgamateFields.CombatsSinceLastRestOrEnchanted.Get(targetCard);
            return Math.Max(0, combatsLeft);
        }
    }
    public override int DisplayAmount => CombatsLeft;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        Amalgamate amalgamate = this;
        CardModel targetCard = amalgamate.Card.DeckVersion ?? amalgamate.Card;
        
        if (player != targetCard.Owner ||
            targetCard.Owner.PlayerCombatState.TurnNumber > 1 ||
            CombatsLeft <= 0)
        {
            GD.Print("Amalgamate: Not applying because either the player is not the card's owner, it's not the first turn, or the number of combats since last rest is too high.");
            return;
        }

        GD.Print($"Amalgamate triggers: {amalgamate.DisplayAmount} combats left");
        CardPileAddResult cardPileAddResult = await CardPileCmd.Add(amalgamate.Card, PileType.Hand);
    }
    
    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (this.Card.DeckVersion != null)
        {
            return Task.CompletedTask;
        }
        int currentCount = AmalgamateFields.CombatsSinceLastRestOrEnchanted.Get(this.Card);
        AmalgamateFields.CombatsSinceLastRestOrEnchanted.Set(this.Card, currentCount + 1);
    
        GD.Print($"Amalgamate: Incremented combat tracker. New count: {currentCount + 1}");
        return Task.CompletedTask;
    }
}