using UnityEngine;

public class EarnGoldCard : LootCard<EarnGoldCardData>
{
    private int stackSize = 0;

    public override void ExecuteLootGetEvent()
    {
        InventoryItem item = Data.BackingItem.CreateClone();
        item.CurrentStackSize = stackSize;

        if (Game.Player.Inventory.TryMoveToInventory(item, true))
        {
            // TODO: message?
        }
        else
        {
            Game.Player.Inventory.DiscardItem(item, false);
        }
    }

    protected override void InitData()
    {
        base.InitData();
        Data.BackingItem = CreateBackingItem();
        stackSize = Random.Range(Data.MinRange, Data.MaxRange);
    }

    protected override string GetCardText()
    {
        return Data.CardText.Replace("$R", stackSize.ToString());
    }

    protected virtual InventoryItem CreateBackingItem()
    {
        return new InventoryItem<EarnGoldCardData>(Data);
    }
}
