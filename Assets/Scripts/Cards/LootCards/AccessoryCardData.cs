using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Item/Accessory Card", order = 1)]
public class AccessoryCardData : EquippableItemCardData
{    
    public override LootCardType LootCardType { get { return LootCardType.Accessory; } }
    public override Type BackingCardType { get { return typeof(AccessoryCard); } }
}
