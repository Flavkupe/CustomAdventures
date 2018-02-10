using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Item Card", order = 1)]
public class ItemCardData : LootCardData
{
    public Sprite Sprite;
    public string Name;
    public InventoryItem BackingItem;
    public int Value = 0;

    public override LootCardType LootCardType { get { return LootCardType.Item; } }
}
