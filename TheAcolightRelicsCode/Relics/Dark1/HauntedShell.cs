using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace TheAcolightRelics.TheAcolightRelicsCode.Relics.Dark1;

public class HauntedShell() : TheAcolightSharedRelics
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (!HauntedShell.CanAffect(card))
            return Task.CompletedTask;
        CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
        return Task.CompletedTask;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom))
            return Task.CompletedTask;
        foreach (CardModel allCard in this.Owner.PlayerCombatState.AllCards)
        {
            if (HauntedShell.CanAffect(allCard))
                CardCmd.ApplyKeyword(allCard, CardKeyword.Ethereal);
        }
        return Task.CompletedTask;
    }

    private static bool CanAffect(CardModel card)
    {
        return card.Rarity == CardRarity.Status && !card.GetKeywordsWithSources(KeywordSources.Local).Contains(CardKeyword.Ethereal);
    }
}