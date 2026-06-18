using System.Diagnostics;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using TheAcolightRelics.TheAcolightRelicsCode.Relics;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics;


public class RainbowInAJar() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(3)
    ];
    
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        RainbowInAJar rainbowInAJar = this;
        if (player != rainbowInAJar.Owner || rainbowInAJar.Owner.PlayerCombatState.TurnNumber > 1)
            return;
        
        // Put a random card from Draw Pile into Hand
        // List<CardModel> list = PileType.Draw.GetPile(player).Cards.ToList<CardModel>();
        // if (list.Count == 0)
        //     return;
        // CardModel card = player.RunState.Rng.CombatCardSelection.NextItem<CardModel>((IEnumerable<CardModel>) list);
        //
        // rainbowInAJar.Flash();
        // CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card, PileType.Hand);

        // Choose 1 of 3 cards in your Draw Pile to add to your Hand.
        // IEnumerable<CardModel> cardOptions = PileType.Draw.GetPile(this.Owner).Cards.ToList<CardModel>().StableShuffle<CardModel>(rainbowInAJar.Owner.RunState.Rng.CombatCardSelection).Take<CardModel>(rainbowInAJar.DynamicVars.Cards.IntValue);
        // CardModel card2 = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(rainbowInAJar.Owner), rainbowInAJar.Owner, new CardSelectorPrefs(rainbowInAJar.SelectionScreenPrompt, 1), (Func<CardModel, bool>) (c => cardOptions.Contains<CardModel>(c)))).FirstOrDefault<CardModel>();
        // if (card2 == null)
        //     ;
        // else
        // {
        //     CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card2, PileType.Hand);
        // }
        
        // At the start of combat put one random card from your Draw Pile to put into your Hand. The card's type is the same as your last played card.

        if (this.LastPlayedCardType != CardType.None)
        {
            await PutRandomCardOfTypeIntoHand(player, this.LastPlayedCardType);
        }
        else
        {
            await PutRandomCardIntoHand(player);
        }
        
        // if (CombatManager.Instance.History.CardPlaysFinished.LastOrDefault() != null)
        // {
        //     CardType lastPlayType = CombatManager.Instance.History.CardPlaysFinished.LastOrDefault().CardPlay.Card.Type;
        //     if (lastPlayType != CardType.None)
        //     {
        //         await PutRandomCardOfTypeIntoHand(player, lastPlayType);
        //     }
        //     else
        //     {
        //         await PutRandomCardIntoHand(player);
        //     }
        // } else 
        // {
        //     await PutRandomCardIntoHand(player);
        // }


        // if (IsAttackSkillOrPower(LastPlayedCardType))
        // {
        //     await PutRandomCardIntoHand(player);
        // }
        // else
        // {
        //     await PutRandomAttackSkillOrPowerIntoHand(player);
        // }
    }

    private async Task PutRandomCardOfTypeIntoHand(Player player, CardType cardType)
    {
        RainbowInAJar rainbowInAJar = this;
        
        List<CardModel> list = PileType.Draw.GetPile(player).Cards
            .Where<CardModel>(new Func<CardModel, bool>(card => card.Type == cardType)).ToList<CardModel>();
        if (list.Count == 0)
            return;
        CardModel card = player.RunState.Rng.CombatCardSelection.NextItem<CardModel>((IEnumerable<CardModel>) list);
        
        rainbowInAJar.Flash();
        GD.Print($"Rainbow In A Jar: Putting {card.Title} into hand.");
        CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card, PileType.Hand);
    }
    
    private async Task PutRandomCardIntoHand(Player player)
    {
        RainbowInAJar rainbowInAJar = this;
        
        List<CardModel> list = PileType.Draw.GetPile(player).Cards.ToList<CardModel>();
        if (list.Count == 0)
            return;
        CardModel card = player.RunState.Rng.CombatCardSelection.NextItem<CardModel>((IEnumerable<CardModel>) list);
        
        rainbowInAJar.Flash();
        GD.Print($"Rainbow In A Jar: Putting random {card.Title} into hand.");
        CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card, PileType.Hand);
    }
    
    // private async Task PutRandomAttackSkillOrPowerIntoHand(Player player)
    // {
    //     RainbowInAJar rainbowInAJar = this;
    //     
    //     List<CardModel> list = PileType.Draw.GetPile(player).Cards
    //         .Where<CardModel>(new Func<CardModel, bool>(rainbowInAJar.FilterAttacksSkillsAndPowers)).ToList<CardModel>();
    //     if (list.Count == 0)
    //         return;
    //     CardModel card = player.RunState.Rng.CombatCardSelection.NextItem<CardModel>((IEnumerable<CardModel>) list);
    //     
    //     rainbowInAJar.Flash();
    //     CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card, PileType.Hand);
    // }
    //
    // private bool Filter(CardModel card)
    // {
    //     bool lastCard = card.Type == LastPlayedCardType;
    //     return lastCard;
    // }
    //
    // private bool FilterAttacksSkillsAndPowers(CardModel card)
    // {
    //     bool lastCard = card.Type == CardType.Attack || card.Type == CardType.Skill || card.Type == CardType.Power;
    //     return lastCard;
    // }
    //
    // private bool IsAttackSkillOrPower(CardType cardType)
    // {
    //     return cardType == CardType.Attack || cardType == CardType.Skill || cardType == CardType.Power;
    // }
    //
    private CardType _lastPlayedCardType = CardType.None;

    [SavedProperty]
    public CardType LastPlayedCardType
    {
        get => this._lastPlayedCardType;
        set
        {
            this.AssertMutable();
            this._lastPlayedCardType = value;
        }
    }

    public override async Task AfterAutoPrePlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        RainbowInAJar rainbowInAJar = this;
    
        if (player != this.Owner || this.Owner.PlayerCombatState.TurnNumber == 1)
        {
            return;
        }

        var lastPlay = CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(); 

        if (lastPlay?.CardPlay?.Card == null)
        {
            return;
        }

        rainbowInAJar.LastPlayedCardType = lastPlay.CardPlay.Card.Type;
    }
}