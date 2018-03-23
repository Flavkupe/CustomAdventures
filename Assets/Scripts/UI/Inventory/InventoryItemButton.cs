using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemButton : MonoBehaviour
{
    public InventoryItem BackingItem;

    public InventoryItemButtonType Type;

    public TextMeshProUGUI StackCount;

    public Image subImage;
    
	// Use this for initialization
	void Start ()
    {
        this.subImage.gameObject.SetActive(false);
        this.StackCount.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
	}

    public void OnButtonClicked()
    {
        if (this.BackingItem != null)
        {
            PlayerInventory inv = Game.Player.Stats.Inventory;
            if (this.BackingItem == inv.EquippedWeapon ||
                this.BackingItem == inv.EquippedArmor ||
                this.BackingItem == inv.EquippedAccessory)
            {
                if (Game.Player.Unequip(this.BackingItem))
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
                    Game.Player.Equip(this.BackingItem);
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

            if (item.CurrentStackSize > 1)
            {
                this.StackCount.gameObject.SetActive(true);
                this.StackCount.text = item.CurrentStackSize.ToString();
            }
            else
            {
                this.StackCount.gameObject.SetActive(false);
            }
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