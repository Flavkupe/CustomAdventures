public abstract class ItemCard<TItemCardDataType> : LootCard<TItemCardDataType> where TItemCardDataType : ItemCardData
{
    public override void ExecuteLootGetEvent()
    {
        InventoryItem item = Data.BackingItem.CloneInstance();
        if (Game.Player.Stats.Inventory.TryMoveToInventory(item, true))
        {
            // TODO: message?
        }
        else
        {
            // TODO: full inventory
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
