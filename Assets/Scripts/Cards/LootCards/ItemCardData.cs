using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Item/Item Card", order = 1)]
public class ItemCardData : LootCardData
{
    public Sprite Sprite;
    public string Name;
    public InventoryItem BackingItem { get; set; }
    public int Value = 0;
    public int MaxStack = 1;

    public override LootCardType LootCardType { get { return LootCardType.Item; } }

    public override Type BackingCardType { get { return typeof(GeneralItemCard); } }
}
