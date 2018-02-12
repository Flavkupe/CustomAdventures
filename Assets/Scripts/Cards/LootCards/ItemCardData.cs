using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Item/Item Card", order = 1)]
public abstract class ItemCardData : LootCardData
{
    public Sprite Sprite;
    public string Name;
    public InventoryItem BackingItem { get; set; }
    public int Value = 0;

    public override LootCardType LootCardType { get { return LootCardType.Item; } }
}
