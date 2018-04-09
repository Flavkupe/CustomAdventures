using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public GridLayoutGroup InventorySlots;

    public InventoryItemButton InventorySlotTemplate;

    private List<InventoryItemButton> inventoryButtons;
    private List<InventoryItemButton> equipmentButtons;
    private List<InventoryItemButton> groundButtons;

    private void Awake()
    {
        equipmentButtons = GetComponentsInChildren<InventoryItemButton>(true).Where(a => a.IsEquipmentType).ToList();
        groundButtons = GetComponentsInChildren<InventoryItemButton>(true).Where(a => a.Type == InventoryItemButtonType.Ground).ToList();
        RefreshInventoryButtons();
    }

    private void RefreshInventoryButtons()
    {
        inventoryButtons = GetComponentsInChildren<InventoryItemButton>(true).Where(a => a.Type == InventoryItemButtonType.Inventory).ToList();        
    }

    public InventoryItemButton GetInventoryItemButton(InventoryItemButtonType type)
    {
        if (type == InventoryItemButtonType.Inventory)
        {
            return inventoryButtons.FirstOrDefault(a => !a.IsOccupied);
        }

        var item = equipmentButtons.FirstOrDefault(a => a.Type == type);
        Debug.Assert(item != null, "Inventory slot not found!");
        return item;
    }

    public void UpdateInventory()
    {
        PlayerInventory inv = Game.Player.Stats.Inventory;

        var slots = InventorySlots.GetComponentsInChildren<InventoryItemButton>(true);
        if (slots.Length < inv.MaxItems)
        {
            for (int i = 0; i < inv.MaxItems - slots.Length; i++)
            {
                var slot = Instantiate(InventorySlotTemplate);
                slot.transform.SetParent(InventorySlots.transform);
            }

            RefreshInventoryButtons();
        }

        EnumUtils.DoForeachEnumValue<InventoryItemButtonType>(slotType =>
        {
            if (slotType != InventoryItemButtonType.Inventory && slotType != InventoryItemButtonType.Ground)
            {
                InventoryItemType itemType = (InventoryItemType)slotType;
                var item = inv.GetEquipmentItem(itemType);
                var slot = GetInventoryItemButton(slotType);
                slot.UpdateItem(item);
            }
        });

        for (int i = 0; i < inventoryButtons.Count; ++i)
        {
            if (inv.InventoryItems.Count > i && inv.InventoryItems[i] != null)
            {
                inventoryButtons[i].UpdateItem(inv.InventoryItems[i]);
            }
            else
            {
                inventoryButtons[i].ClearItem();
            }
        }

        groundButtons.ForEach(a => {
            a.ClearItem();
            a.gameObject.SetActive(false);
        });

        var groundItems = Game.Dungeon.GetGroundItems();
        for (int i = 0; i < groundItems.Count; ++i)
        {
            if (i >= groundButtons.Count)
            {
                break;
            }

            var button = groundButtons[i];
            button.gameObject.SetActive(true);
            button.UpdateItem(groundItems[i].Item);
        }
    }
}
