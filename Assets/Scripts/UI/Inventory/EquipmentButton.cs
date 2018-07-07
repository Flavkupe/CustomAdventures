using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class EquipmentButton : InventoryItemButton
{
    protected override void OnItemPlacedHere(InventoryItem item)
    {
        // TODO: LEFT OFF HERE
        PlayerInventory inv = Game.Player.Inventory;
        if (item.Type == this.Type)
        inv.EquipInventoryItemDirectly(item);
    }
}

