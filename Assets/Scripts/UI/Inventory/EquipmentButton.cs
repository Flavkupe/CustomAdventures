using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class EquipmentButton : InventoryItemButton
{
    protected override void OnItemPlacedHere(InventoryItem item)
    {
        if (!CanHoldItemType(item))
        {
            Debug.Assert(false, "Trying to equip wrong item type!");
            return;
        }

        PlayerInventory inv = Game.Player.Inventory;
        inv.EquipInventoryItemDirectly(item);
        item.ItemEquipped();
    }

    protected override bool CanHoldItemType(InventoryItem item)
    {
        return item.Type == (InventoryItemType)Type;
    }
}

