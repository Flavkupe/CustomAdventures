using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public GridLayoutGroup InventorySlots;

    public InventoryItemButton InventorySlotTemplate;

    private InventoryItemButton weaponButton;

    private InventoryItemButton armorButton;

    private InventoryItemButton accessoryButton;

    private List<InventoryItemButton> inventoryButtons;

    private void Awake()
    {
        weaponButton = GetComponentsInChildren<InventoryItemButton>().FirstOrDefault(a => a.Type == InventoryItemButtonType.Weapon);
        armorButton = GetComponentsInChildren<InventoryItemButton>().FirstOrDefault(a => a.Type == InventoryItemButtonType.Armor);
        accessoryButton = GetComponentsInChildren<InventoryItemButton>().FirstOrDefault(a => a.Type == InventoryItemButtonType.Accessory);
        RefreshInventoryButtons();
    }

    private void RefreshInventoryButtons()
    {
        inventoryButtons = GetComponentsInChildren<InventoryItemButton>().Where(a => a.Type == InventoryItemButtonType.Inventory).ToList();
    }

    // Use this for initialization
    private void Start ()
    {
    }

    // Update is called once per frame
    private void Update ()
    {
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

        weaponButton.UpdateItem(inv.EquippedWeapon);
        armorButton.UpdateItem(inv.EquippedArmor);
        accessoryButton.UpdateItem(inv.EquippedAccessory);
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
    }
}
