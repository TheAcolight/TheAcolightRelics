using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace TheAcolightRelics.TheAcolightRelicsCode.Fields;

public class AmalgamateFields
{
    public static readonly SavedSpireField<CardModel, int> CombatsSinceLastRestOrEnchanted = new(() => 0, "amalgamate_combats_since_rest");
    public static readonly SavedSpireField<CardModel, bool> AmalgamateNewEnchantment = new(() => true, "amalgamate_new_enchantment");
}