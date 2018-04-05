using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemButton : MonoBehaviour
{
    public InventoryItem BackingItem;

    public InventoryItemButtonType Type;

    public TextMeshProUGUI StackCount;

    public Image subImage;

    public bool IsOccupied { get { return BackingItem != null; } }

    // Use this for initialization
    private void Start ()
    {
        subImage.gameObject.SetActive(false);
        StackCount.gameObject.SetActive(false);
    }

    public void OnButtonClicked()
    {
        if (BackingItem != null)
        {
            PlayerInventory inv = Game.Player.Stats.Inventory;
            if (BackingItem.IsEquipment)
            {
                if (BackingItem == inv.GetEquipmentItem(BackingItem.Type))
                {
                    if (Game.Player.Stats.Inventory.Unequip(BackingItem.Type))
                    {
                        BackingItem = null;
                    }
                }
                else
                {
                    Game.Player.Stats.Inventory.Equip(BackingItem);
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
    Inventory = 1,
    Weapon = 2,
    Armor = 3,
    Accessory = 4,
    Boots = 5,
    Shield = 6,
    Helmet = 7,
    Ring = 8,
}