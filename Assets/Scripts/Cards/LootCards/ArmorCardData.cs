using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Item/Armor Card", order = 1)]
public class ArmorCardData : EquippableItemCardData
{
    public int Blocking = 1;
    public int Durability = 5;
    public override LootCardType LootCardType { get { return LootCardType.Armor; } }
    public override Type BackingCardType { get { return typeof(ArmorCard); } }
}
