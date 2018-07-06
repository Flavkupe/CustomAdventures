using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Item/Item Card", order = 1)]
public class ItemCardData : LootCardData
{
    public Sprite Sprite;
    public InventoryItem BackingItem { get; set; }
    public int Value = 0;
    public int MaxStack = 1;

    [Tooltip("How scaled the object will be when dropped on ground")]
    public float ScaleOnGround = 0.50f;

    public InventoryItemType ItemType = InventoryItemType.Misc;

    public override LootCardType LootCardType => LootCardType.Item;

    public override Type BackingCardType => typeof(GeneralItemCard);

    public AudioClip CustomPickupSound;
    public AudioClip CustomDropSound;

    public virtual InventoryItem CreateInventoryItem()
    {
        return new InventoryItem<ItemCardData>(this);
    }
}

public class EquippableItemCardData : ItemCardData
{
    public AudioClip[] EquipSounds;
    public AudioClip[] BreakSounds;
}
