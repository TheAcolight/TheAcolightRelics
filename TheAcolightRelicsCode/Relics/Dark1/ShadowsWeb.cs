using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using TheAcolightRelics.TheAcolightRelicsCode.Relics;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics.Dark1;

public class ShadowsWeb() : TheAcolightSharedRelics
{
        public override RelicRarity Rarity =>
        RelicRarity.Rare;
    
  private bool _anySkillsPlayedThisTurn;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new PowerVar<WeakPower>(1)
  ];

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
    if (this.Owner != cardPlay.Card.Owner || !CombatManager.Instance.IsInProgress ||
        (cardPlay.Card.Type != CardType.Skill))
      return Task.CompletedTask;
    this.Status = RelicStatus.Normal;
    this.AnySkillsPlayedThisTurn = true;
    return Task.CompletedTask;
  }

  public override async Task AfterSideTurnEnd(
    PlayerChoiceContext choiceContext,
    CombatSide side,
    IEnumerable<Creature> participants)
  {
    if (!participants.Contains<Creature>(this.Owner.Creature) || this.AnySkillsPlayedThisTurn)
    {
      this.AnySkillsPlayedThisTurn = false;
      return; 
    }

    ShadowsWeb shadowsWeb = this;
    
    // If the player didn't play any skills this turn...
    IReadOnlyList<WeakPower> weakPowerList = await PowerCmd.Apply<WeakPower>(choiceContext, (IEnumerable<Creature>) shadowsWeb.Owner.Creature.CombatState.HittableEnemies, shadowsWeb.DynamicVars.Weak.BaseValue, shadowsWeb.Owner.Creature, (CardModel) null);
    
    // Reset after triggering the effect
    this.AnySkillsPlayedThisTurn = false;
  }
  

  public override Task AfterCombatEnd(CombatRoom _)
  {
    this.Status = RelicStatus.Normal;
    this.AnySkillsPlayedThisTurn = false;
    return Task.CompletedTask;
  }
}