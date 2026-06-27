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

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics.Light1;


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

        // Get list of attacks
        IEnumerable<CardModel> attackOptions = PileType.Draw.GetPile(rainbowInAJar.Owner).Cards.ToList<CardModel>().StableShuffle<CardModel>(rainbowInAJar.Owner.RunState.Rng.CombatCardSelection).Where<CardModel>((Func<CardModel, bool>) (c => c.Type == CardType.Attack));
        // Get list of skills
        IEnumerable<CardModel> skillOptions = PileType.Draw.GetPile(rainbowInAJar.Owner).Cards.ToList<CardModel>().StableShuffle<CardModel>(rainbowInAJar.Owner.RunState.Rng.CombatCardSelection).Where<CardModel>((Func<CardModel, bool>) (c => c.Type == CardType.Skill));
        // Get list of powers
        IEnumerable<CardModel> powerOptions = PileType.Draw.GetPile(rainbowInAJar.Owner).Cards.ToList<CardModel>().StableShuffle<CardModel>(rainbowInAJar.Owner.RunState.Rng.CombatCardSelection).Where<CardModel>((Func<CardModel, bool>) (c => c.Type == CardType.Power));
        // Get list of one of each

        var cardOptions = new List<CardModel>();

        var attack = attackOptions.FirstOrDefault();
        var skill = skillOptions.FirstOrDefault();
        var power = powerOptions.FirstOrDefault();

        if (attack is not null) cardOptions.Add(attack);
        if (skill is not null)  cardOptions.Add(skill);
        if (power is not null)  cardOptions.Add(power);

        CardModel card2 = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(rainbowInAJar.Owner), rainbowInAJar.Owner, new CardSelectorPrefs(rainbowInAJar.SelectionScreenPrompt, 1), (Func<CardModel, bool>) (c => cardOptions.Contains<CardModel>(c)))).FirstOrDefault<CardModel>();
        if (card2 == null)
            ;
        else
        {
            CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card2, PileType.Hand);
        }
        rainbowInAJar.Flash();

    }
}