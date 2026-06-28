using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using TheAcolightRelics.TheAcolightRelicsCode.Relics;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics.Dark1;

public class DarkBlade() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity => RelicRarity.Common;
    
    private bool _anyAttacksPlayedThisTurn;
    private bool _anySkillsPlayedThisTurn;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6, ValueProp.Unpowered),
        new BlockVar(5, ValueProp.Unpowered)
    ];

    private bool AnyAttacksPlayedThisTurn
    {
        get => this._anyAttacksPlayedThisTurn;
        set
        {
            this.AssertMutable();
            this._anyAttacksPlayedThisTurn = value;
        }
    }

    private bool AnySkillsPlayedThisTurn
    {
        get => this._anySkillsPlayedThisTurn;
        set
        {
            this.AssertMutable();
            this._anySkillsPlayedThisTurn = value;
        }
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (this.Owner != cardPlay.Card.Owner || !CombatManager.Instance.IsInProgress)
            return Task.CompletedTask;

        if (cardPlay.Card.Type == CardType.Attack)
        {
            this.Status = RelicStatus.Normal;
            this.AnyAttacksPlayedThisTurn = true;
        }
        else if (cardPlay.Card.Type == CardType.Skill)
        {
            this.Status = RelicStatus.Normal;
            this.AnySkillsPlayedThisTurn = true;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        // If it's not the player's turn ending, safely reset and exit
        if (!participants.Contains<Creature>(this.Owner.Creature))
        {
            this.AnyAttacksPlayedThisTurn = false;
            this.AnySkillsPlayedThisTurn = false;
            return;
        }
        
        DarkBlade darkBlade = this;

        // Effect 1: If no attacks were played this turn, deal 6 damage to a random enemy
        if (!this.AnyAttacksPlayedThisTurn)
        {
            Creature target = darkBlade.Owner.RunState.Rng.CombatTargets.NextItem<Creature>((IEnumerable<Creature>) darkBlade.Owner.Creature.CombatState.HittableEnemies);
            
            if (target != null)
            {
                IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext, target, darkBlade.DynamicVars.Damage, darkBlade.Owner.Creature);
            }
        }

        // Effect 2: If no skills were played this turn, gain 5 block
        if (!this.AnySkillsPlayedThisTurn)
        {
            Decimal num = await CreatureCmd.GainBlock(darkBlade.Owner.Creature, darkBlade.DynamicVars.Block, (CardPlay) null);
        }
        
        // Reset both flags at the end of the turn
        this.AnyAttacksPlayedThisTurn = false;
        this.AnySkillsPlayedThisTurn = false;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        this.Status = RelicStatus.Normal;
        this.AnyAttacksPlayedThisTurn = false;
        this.AnySkillsPlayedThisTurn = false;
        return Task.CompletedTask;
    }
}