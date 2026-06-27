using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using TheAcolightRelics.TheAcolightRelicsCode.Relics;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics.Light1;


public class TemperedGlass() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("PlatingPower", 7)
    ];
    
    private bool _tookDamageDuringCombat;
    
    [SavedProperty]
    public bool TookDamageDuringCombat
    {
        get => this._tookDamageDuringCombat;
        set
        {
            this.AssertMutable();
            this._tookDamageDuringCombat = value;
        }
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return;        
        
        if (!this.TookDamageDuringCombat)
        {
            TemperedGlass temperedGlass = this;
            temperedGlass.Flash();
            PlatingPower platingPower = await PowerCmd.Apply<PlatingPower>((PlayerChoiceContext) new ThrowingPlayerChoiceContext(), temperedGlass.Owner.Creature, temperedGlass.DynamicVars["PlatingPower"].BaseValue, temperedGlass.Owner.Creature, (CardModel) null);
        }

        this.TookDamageDuringCombat = false;
        return;
    }

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
        this.TookDamageDuringCombat = true;
        return Task.CompletedTask;
    }
}