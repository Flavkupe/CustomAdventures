using System.Collections;

public abstract class ItemCard<TItemCardDataType> : LootCard<TItemCardDataType> where TItemCardDataType : ItemCardData
{
    protected override IEnumerator ExecuteGetLootEvent(LootCardExecutionContext context)
    {
        InventoryItem item = Data.BackingItem.CreateClone();
        if (context.Player.Inventory.TryMoveToInventory(item, true))
        {
            item.ItemLooted();
        }
        else
        {
            item.ItemDropped();
            context.Player.Inventory.DiscardItem(item, false);
        }

        yield return null;
    }

    protected override void InitData()
    {
        base.InitData();
        Data.BackingItem = CreateBackingItem();
    }    

    protected virtual InventoryItem CreateBackingItem()
    {
        return new InventoryItem<TItemCardDataType>(Data);
    }
}
