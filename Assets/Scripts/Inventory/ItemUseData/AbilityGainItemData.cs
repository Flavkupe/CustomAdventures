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

        foreach (var ability in AbilitiesToAdd)
        {
            var card = ability.CreateCard<IAbilityCard>();
            context.Dungeon.Decks.AbilityDeck.PushCard(card);
        }

        return true;
    }
}

