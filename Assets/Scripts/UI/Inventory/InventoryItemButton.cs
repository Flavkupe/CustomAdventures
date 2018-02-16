using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemButton : MonoBehaviour
{
    public InventoryItem BackingItem;

    public InventoryItemButtonType Type;

    public Image subImage;
    
	// Use this for initialization
	void Start ()
    {
        this.subImage.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
	}

    public void OnButtonClicked()
    {
        if (this.BackingItem != null)
        {
            PlayerInventory inv = Player.Instance.Stats.Inventory;
            if (this.BackingItem == inv.EquippedWeapon ||
                this.BackingItem == inv.EquippedArmor ||
                this.BackingItem == inv.EquippedAccessory)
            {
                if (Player.Instance.Unequip(this.BackingItem))
                {
                    this.BackingItem = null;
                }
            }
            else
            {
                if ((this.BackingItem.Type == InventoryItemType.Weapon) ||
                    (this.BackingItem.Type == InventoryItemType.Armor) ||
                    (this.BackingItem.Type == InventoryItemType.Accessory))
                {
                    Player.Instance.Equip(this.BackingItem);
                } 
            }
        }
    }

    public void ClearItem()
    {
        this.BackingItem = null;
        this.subImage.sprite = null;
        this.subImage.gameObject.SetActive(false);
    }

    public void UpdateItem(InventoryItem item)
    {
        this.BackingItem = item;
        if (this.BackingItem != null)
        {
            this.subImage.gameObject.SetActive(true);
            this.subImage.sprite = this.BackingItem.ItemData.Sprite;
        }        
        else
        {
            this.ClearItem();
        }
    }
}

public enum InventoryItemButtonType
{
    Inventory,
    Weapon,
    Armor,
    Accessory,
}