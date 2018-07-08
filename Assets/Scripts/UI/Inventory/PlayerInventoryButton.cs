using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerInventoryButton : InventoryItemButton
{
    protected override void OnItemRemovedFromHere()
    {
        if (BackingItem != null)
        {
            PlayerInventory inv = Game.Player.Inventory;
            inv.ClearInventoryItem(BackingItem, false);
        }
    }

    protected override void OnItemPlacedHere(InventoryItem item)
    {
        PlayerInventory inv = Game.Player.Inventory;
        inv.TryMoveToInventory(item, false, false);
    }

    protected override bool CanHoldItemType(InventoryItem item)
    {
        return true;
    }
}

