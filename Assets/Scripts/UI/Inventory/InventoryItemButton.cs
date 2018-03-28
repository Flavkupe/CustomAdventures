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
    private void Start ()
    {
        subImage.gameObject.SetActive(false);
        StackCount.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update ()
    {
	}

    public void OnButtonClicked()
    {
        if (BackingItem != null)
        {
            PlayerInventory inv = Game.Player.Stats.Inventory;
            if (BackingItem == inv.EquippedWeapon ||
                BackingItem == inv.EquippedArmor ||
                BackingItem == inv.EquippedAccessory)
            {
                if (Game.Player.Unequip(BackingItem))
                {
                    BackingItem = null;
                }
            }
            else
            {
                if ((BackingItem.Type == InventoryItemType.Weapon) ||
                    (BackingItem.Type == InventoryItemType.Armor) ||
                    (BackingItem.Type == InventoryItemType.Accessory))
                {
                    Game.Player.Equip(BackingItem);
                } 
            }
        }
    }

    public void ClearItem()
    {
        BackingItem = null;
        subImage.sprite = null;
        subImage.gameObject.SetActive(false);
    }

    public void UpdateItem(InventoryItem item)
    {
        BackingItem = item;
        if (BackingItem != null)
        {
            subImage.gameObject.SetActive(true);
            subImage.sprite = BackingItem.ItemData.Sprite;

            if (item.CurrentStackSize > 1)
            {
                StackCount.gameObject.SetActive(true);
                StackCount.text = item.CurrentStackSize.ToString();
            }
            else
            {
                StackCount.gameObject.SetActive(false);
            }
        }        
        else
        {
            ClearItem();
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