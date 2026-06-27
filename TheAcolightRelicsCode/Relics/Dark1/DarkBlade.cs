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
    public override RelicRarity Rarity =>
        RelicRarity.Common;
    
  private bool _anyAttacksOrSkillsPlayedThisTurn;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DamageVar(6, ValueProp.Unpowered),
    new BlockVar(5, ValueProp.Unpowered)
  ];

  private bool AnyAttacksOrSkillsPlayedThisTurn
  {
    get => this._anyAttacksOrSkillsPlayedThisTurn;
    set
    {
      this.AssertMutable();
      this._anyAttacksOrSkillsPlayedThisTurn = value;
    }
  }

  public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (this.Owner != cardPlay.Card.Owner || !CombatManager.Instance.IsInProgress ||
        (cardPlay.Card.Type != CardType.Attack && cardPlay.Card.Type != CardType.Skill))
      return Task.CompletedTask;
    this.Status = RelicStatus.Normal;
    this.AnyAttacksOrSkillsPlayedThisTurn = true;
    return Task.CompletedTask;
  }

  public override async Task AfterSideTurnEnd(
    PlayerChoiceContext choiceContext,
    CombatSide side,
    IEnumerable<Creature> participants)
  {
    if (!participants.Contains<Creature>(this.Owner.Creature) || this.AnyAttacksOrSkillsPlayedThisTurn)
    {
      this.AnyAttacksOrSkillsPlayedThisTurn = false;
      return;
    }
    
    // If the player didn't play any attacks or skills this turn...
    // Deal 6 damage to a random enemy
    DarkBlade darkBlade = this;
    Creature target = darkBlade.Owner.RunState.Rng.CombatTargets.NextItem<Creature>((IEnumerable<Creature>) darkBlade.Owner.Creature.CombatState.HittableEnemies);
    if (target == null)
      return;
    IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext, target, darkBlade.DynamicVars.Damage, darkBlade.Owner.Creature);
    // Gain 5 block
    Decimal num = await CreatureCmd.GainBlock(darkBlade.Owner.Creature, darkBlade.DynamicVars.Block, (CardPlay) null);
    
    // Reset after triggering the effect
    this.AnyAttacksOrSkillsPlayedThisTurn = false;
  }
  

  public override Task AfterCombatEnd(CombatRoom _)
  {
    this.Status = RelicStatus.Normal;
    this.AnyAttacksOrSkillsPlayedThisTurn = false;
    return Task.CompletedTask;
  }
}