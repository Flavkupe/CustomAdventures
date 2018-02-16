
using UnityEngine;

public class GeneralItemCard : ItemCard<ItemCardData>
{
    protected override InventoryItem<ItemCardData> AddBackingComponent(GameObject obj)
    {
        return obj.AddComponent<GeneralItem>();
    }
}
