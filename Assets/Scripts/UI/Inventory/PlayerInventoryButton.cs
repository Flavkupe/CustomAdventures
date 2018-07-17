using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

public class PlayerInventoryButton : InventoryItemButton, IPointerClickHandler
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

    public void OnPointerClick(PointerEventData e)
    {
        if (BackingItem != null)
        {            
            if (e.button == PointerEventData.InputButton.Right)
            {
                RightClickItem();
            }
        }
    }

    private void RightClickItem()
    {
        if (BackingItem != null)
        {
            // TODO: singletons
            var inventory = Game.Player.Inventory.InventoryItems;
            if (BackingItem.ItemUsed(new ItemUseContext(Game.Player, Game.Dungeon, inventory)))
            {
                Game.UI.UpdateInventory();
            }
        }
    }
}

