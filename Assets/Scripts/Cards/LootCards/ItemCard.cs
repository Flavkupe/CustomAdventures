public abstract class ItemCard<TItemCardDataType> : LootCard<TItemCardDataType> where TItemCardDataType : ItemCardData
{
    public override void ExecuteLootGetEvent()
    {
        InventoryItem item = Data.BackingItem.CreateClone();
        if (Game.Player.Stats.Inventory.TryMoveToInventory(item, true))
        {
            item.ItemLooted();
        }
        else
        {
            item.ItemDropped();
            Game.Player.Stats.Inventory.DiscardItem(item, false);
        }
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
