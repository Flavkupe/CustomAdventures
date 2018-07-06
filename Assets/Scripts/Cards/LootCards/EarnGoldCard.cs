using System.Collections;
using UnityEngine;

public class EarnGoldCard : LootCard<EarnGoldCardData>
{
    private int stackSize = 0;

    protected override IEnumerator ExecuteGetLootEvent(LootCardExecutionContext context)
    {
        InventoryItem item = Data.BackingItem.CreateClone();
        item.CurrentStackSize = stackSize;

        if (context.Player.Inventory.TryMoveToInventory(item, true))
        {
            // TODO: message?
        }
        else
        {
            context.Player.Inventory.DiscardItem(item, false);
        }

        yield return null;
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
        return Data.CreateInventoryItem();
    }
}
