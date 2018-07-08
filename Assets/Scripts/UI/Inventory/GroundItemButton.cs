using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class GroundItemButton : InventoryItemButton
{
    protected override void OnItemPlacedHere(InventoryItem item)
    {
        PlayerInventory inv = Game.Player.Inventory;
        inv.DiscardItem(item);
    }

    protected override void OnItemRemovedFromHere()
    {
        if (BackingItem != null)
        {
            var grid = Game.Dungeon.Grid;
            var player = Game.Player;

            if (grid.TryClearTileItem(player.XCoord, player.YCoord, BackingItem))
            {
                BackingItem = null;
            }
        }
    }

    protected override bool CanHoldItemType(InventoryItem item)
    {
        // Can throw away any item
        return true;
    }

    public override bool IsGroundItem => true;
}

