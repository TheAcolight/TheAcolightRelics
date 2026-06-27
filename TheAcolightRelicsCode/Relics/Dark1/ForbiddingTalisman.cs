using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TheAcolightRelics.TheAcolightRelicsCode.Relics;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics.Dark1;

public class ForbiddingTalisman() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Power",2), 
        new DynamicVar("Turn",2)
    ];
    
    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        ForbiddingTalisman forbiddingTalisman = this;
        if (!participants.Contains<Creature>(forbiddingTalisman.Owner.Creature) || forbiddingTalisman.Owner.PlayerCombatState.TurnNumber != 2)
            return;
        forbiddingTalisman.Flash();
        IReadOnlyList<VulnerablePower> vulnerablePowerList = await PowerCmd.Apply<VulnerablePower>(choiceContext, (IEnumerable<Creature>) combatState.HittableEnemies, forbiddingTalisman.DynamicVars["Power"].BaseValue, forbiddingTalisman.Owner.Creature, (CardModel) null);
        IReadOnlyList<WeakPower> weakPowerList = await PowerCmd.Apply<WeakPower>(choiceContext, (IEnumerable<Creature>) combatState.HittableEnemies, forbiddingTalisman.DynamicVars["Power"].BaseValue, forbiddingTalisman.Owner.Creature, (CardModel) null);

    }
}