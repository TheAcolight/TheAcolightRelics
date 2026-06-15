using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using TheAcolightRelics.TheAcolightRelicsCode.Relics;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics;

public class PinkCrystal() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HealVar(7)
    ];
    // Heal 7 HP Upon Pickup
    public override bool HasUponPickupEffect => true;
    public override async Task AfterObtained()
    {
        PinkCrystal pinkCrystal = this;
        await CreatureCmd.Heal(pinkCrystal.Owner.Creature, pinkCrystal.DynamicVars.Heal.BaseValue);
    }
    
    private int _damageTakenThisCombat = 0;
    
    [SavedProperty]
    public int DamageTakenThisCombat
    {
        get => this._damageTakenThisCombat;
        set
        {
            this.AssertMutable();
            this._damageTakenThisCombat = value;
        }
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        this.DamageTakenThisCombat = 0;
        return Task.CompletedTask;
    }
    // If you lose less than 7 HP during a combat, heal the amount lost at the end of combat.
    public override Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!(this.Owner.RunState.CurrentRoom is CombatRoom) || target != this.Owner.Creature || result.UnblockedDamage <= 0 || props.HasFlag((Enum) ValueProp.Unblockable))
            return Task.CompletedTask;
        this.DamageTakenThisCombat += result.UnblockedDamage;
        
        return Task.CompletedTask;
    }
    
    public override async Task AfterCombatVictory(CombatRoom _)
    {
        PinkCrystal pinkCrystal = this;
        if (pinkCrystal.Owner.Creature.IsDead || DamageTakenThisCombat > pinkCrystal.DynamicVars.Heal.BaseValue)
            return;
        pinkCrystal.Flash();
        await CreatureCmd.Heal(pinkCrystal.Owner.Creature, pinkCrystal._damageTakenThisCombat);
    }
}