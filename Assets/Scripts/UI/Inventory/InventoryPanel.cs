using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryPanel : MonoBehaviour {


    private InventoryItemButton weaponButton;

    private InventoryItemButton armorButton;

    private InventoryItemButton accessoryButton;

    private List<InventoryItemButton> inventoryButtons;

    private void Awake()
    {
        weaponButton = GetComponentsInChildren<InventoryItemButton>().FirstOrDefault(a => a.Type == InventoryItemButtonType.Weapon);
        armorButton = GetComponentsInChildren<InventoryItemButton>().FirstOrDefault(a => a.Type == InventoryItemButtonType.Armor);
        accessoryButton = GetComponentsInChildren<InventoryItemButton>().FirstOrDefault(a => a.Type == InventoryItemButtonType.Accessory);
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
