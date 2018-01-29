using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class InventoryPanel : MonoBehaviour {


    private InventoryItemButton weaponButton;

    private InventoryItemButton armorButton;

    private InventoryItemButton accessoryButton;

    private List<InventoryItemButton> inventoryButtons;

    void Awake()
    {
        weaponButton = this.GetComponentsInChildren<InventoryItemButton>().FirstOrDefault(a => a.Type == InventoryItemButtonType.Weapon);
        armorButton = this.GetComponentsInChildren<InventoryItemButton>().FirstOrDefault(a => a.Type == InventoryItemButtonType.Armor);
        accessoryButton = this.GetComponentsInChildren<InventoryItemButton>().FirstOrDefault(a => a.Type == InventoryItemButtonType.Accessory);
        inventoryButtons = this.GetComponentsInChildren<InventoryItemButton>().Where(a => a.Type == InventoryItemButtonType.Inventory).ToList();
    }

    // Use this for initialization
    void Start ()
    {
    } 

    // Update is called once per frame
    void Update ()
    {		
	}

    public void UpdateInventory()
    {
        PlayerInventory inv = Player.Instance.Stats.Inventory;
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
