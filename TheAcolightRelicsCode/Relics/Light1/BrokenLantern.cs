using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics.Light1;

public class BrokenLantern() : TheAcolightSharedRelics
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2)
    ];
    
    public override RelicRarity Rarity =>
        RelicRarity.Common;

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        BrokenLantern lantern = this;
        if (!participants.Contains<Creature>(lantern.Owner.Creature) || lantern.Owner.PlayerCombatState.TurnNumber > 1)
        {
            if (lantern.Owner.PlayerCombatState.TurnNumber == 2)
            {
                lantern.Flash();
                await PlayerCmd.LoseEnergy(1, lantern.Owner);
            }
            return;
        }
        lantern.Flash();
        await PlayerCmd.GainEnergy(lantern.DynamicVars.Energy.BaseValue, lantern.Owner);
    }
}