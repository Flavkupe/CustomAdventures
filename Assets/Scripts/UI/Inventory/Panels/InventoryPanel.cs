using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public GridLayoutGroup InventorySlots;
    public GridLayoutGroup GroundSlots;

    public PlayerInventoryButton InventorySlotTemplate;
    public GroundItemButton GroundSlotTemplate;

    private List<InventoryItemButton> inventoryButtons;
    private List<InventoryItemButton> equipmentButtons;
    private List<InventoryItemButton> groundButtons;

    private void Awake()
    {
        equipmentButtons = GetComponentsInChildren<InventoryItemButton>(true).Where(a => a.IsEquipmentType).ToList();
        RefreshGroundbuttons();
        RefreshInventoryButtons();
    }

    private void RefreshGroundbuttons()
    {
        groundButtons = GetComponentsInChildren<InventoryItemButton>(true).Where(a => a.Type == InventoryItemButtonType.Ground).ToList();
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
        PlayerInventory inv = Game.Player.Inventory;

        var slots = InventorySlots.GetComponentsInChildren<InventoryItemButton>(true);
        if (slots.Length < inv.MaxItems)
        {
            for (int i = 0; i < inv.MaxItems - slots.Length; i++)
            {
                var slot = Instantiate(InventorySlotTemplate);
                ParentTo(slot.transform, InventorySlots.transform);
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
            if (inv.InventoryItems.Count > i && inv.InventoryItems.Get(i) != null)
            {
                inventoryButtons[i].UpdateItem(inv.InventoryItems.Get(i));
            }
            else
            {
                inventoryButtons[i].ClearItem();
            }
        }

        UpdateGroundItemSlots();
    }

    private void UpdateGroundItemSlots()
    {
        RefreshGroundbuttons();

        groundButtons.ForEach(a =>
        {
            a.ClearItem();
            a.gameObject.SetActive(false);
        });

        var groundItems = Game.Dungeon.GetGroundItems();
        for (int i = 0; i < groundItems.Count; ++i)
        {
            if (i >= groundButtons.Count)
            {
                // Create new buttons as needed
                var newButton = Instantiate(GroundSlotTemplate);
                ParentTo(newButton.transform, GroundSlots.transform);
                newButton.Type = InventoryItemButtonType.Ground;
                groundButtons.Add(newButton);
            }

            var button = groundButtons[i];
            button.gameObject.SetActive(true);
            button.UpdateItem(groundItems.Get(i).Item);
        }

        // Destroy extra buttons
        for (int i = groundButtons.Count - 1; i > groundItems.Count - 1; i--)
        {
            var button = groundButtons[i];
            groundButtons.Remove(button);
            button.DestroyButton();
        }        
    }

    private void ParentTo(Transform item, Transform parent)
    {
        item.SetParent(parent, false);
        item.localScale = new Vector3(1.0f, 1.0f);
        var rect = item.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition3D = rect.anchoredPosition3D.SetZ(0.0f);
        }
    }
}
