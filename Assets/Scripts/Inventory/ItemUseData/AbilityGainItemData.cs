using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Usage", menuName = "Create Cards/Item Use Data/Ability Gain", order = 1)]
public class AbilityGainItemData : ItemUseData
{
    public AbilityCardData[] AbilitiesToAdd;

    public override bool ItemUsed(InventoryItem item, ItemUseContext context)
    {
        if (AbilitiesToAdd.Length == 0)
        {
            return false;
        }

        if (ItemUsedSounds.Length > 0)
        {
            context.Player.PlaySounds(ItemUsedSounds);
        }

        var newCards = new List<IAbilityCard>();
        foreach (var ability in AbilitiesToAdd)
        {
            newCards.Add(ability.CreateCard<IAbilityCard>());
        }

        var abilityDeck = context.Dungeon.Decks.AbilityDeck;
        context.Dungeon.ShuffleNewCardsIntoDeck(context.Dungeon.Decks.AbilityDeck, newCards);

        return true;
    }
}

