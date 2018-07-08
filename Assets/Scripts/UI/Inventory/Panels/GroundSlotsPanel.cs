using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class GroundSlotsPanel : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            var source = eventData.pointerDrag.GetComponent<InventoryItemButton>();
            if (source == null || source.IsGroundItem || !source.IsOccupied)
            {
                return;
            }

            var inv = Game.Player.Inventory;
            if (source.IsEquipmentType)
            {
                inv.DiscardEquipment(source.BackingItem);
            }
            else
            {
                inv.DiscardItem(source.BackingItem);
            }
        }
    }
}
