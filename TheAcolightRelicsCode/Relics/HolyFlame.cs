using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using TheAcolightRelics.TheAcolightRelicsCode.Relics;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics;
public class HolyFlame() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(1M, ValueProp.Unpowered),
        new DynamicVar("Increase", 2M)
    ];

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        this.DynamicVars.Damage.BaseValue = 1M;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        HolyFlame holyFlame = this;
        holyFlame.Flash();
        Creature target = holyFlame.Owner.RunState.Rng.CombatTargets.NextItem<Creature>((IEnumerable<Creature>) holyFlame.Owner.Creature.CombatState.HittableEnemies);
        if (target == null)
            return;
        VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_coin_explosion_jumbo");
        SfxCmd.Play("event:/sfx/characters/attack_fire");
        IEnumerable<DamageResult> damageResults = await CreatureCmd.Damage(choiceContext, target, holyFlame.DynamicVars.Damage, holyFlame.Owner.Creature);
        
        holyFlame.DynamicVars.Damage.BaseValue += holyFlame.DynamicVars["Increase"].BaseValue;
    }
    
}