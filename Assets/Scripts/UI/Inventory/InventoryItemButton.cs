using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemButton : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem BackingItem;

    public InventoryItemButtonType Type;

    public TextMeshProUGUI StackCount;

    public Image subImage;

    public Image DurabilitySlider;

    public bool IsOccupied => BackingItem != null;

    public bool IsEquipmentType => Type != InventoryItemButtonType.Inventory && Type != InventoryItemButtonType.Ground;

    public void OnPointerClick(PointerEventData e)
    {
        //if (BackingItem != null)
        //{            
        //    if (e.button == PointerEventData.InputButton.Left)
        //    {
        //        LeftClickItem();
        //    }
        //    else if (e.button == PointerEventData.InputButton.Right)
        //    {
        //        RightClickItem();
        //    }
        //}
    }

    private CursorObject _dragObject;

    public void OnStartDrag(PointerEventData data)
    {
        if (BackingItem != null)
        {
            _dragObject = Utils.InstantiateOfType<CursorObject>("drag");
            _dragObject.transform.SetParent(Game.UI.MainCanvas.transform);
            _dragObject.SetImage(BackingItem.ItemData.Sprite, subImage.rectTransform);
            subImage.gameObject.SetActive(false);
            data.selectedObject = gameObject;
            Cursor.visible = false;
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        Cursor.visible = true;
        if (_dragObject != null)
        {
            subImage.gameObject.SetActive(true);
            Destroy(_dragObject.gameObject);
            _dragObject = null;
        }
    }

    public void OnDrop(PointerEventData data)
    {
        if (data.selectedObject != null)
        {
            var button = data.selectedObject.GetComponent<InventoryItemButton>();
            if (button != null)
            {
                DropItemHere(button);
            }
        }
    }

    protected virtual void DropItemHere(InventoryItemButton source)
    {
        var otherItem = source.BackingItem;
        if (BackingItem != null)
        {
            // Swap with other
            source.OnItemPlacedHere(BackingItem);            
        }
        else
        {
            source.ClearItem();
        }

        OnItemPlacedHere(otherItem);
    }

    protected virtual void OnItemPlacedHere(InventoryItem item)
    {
        PlayerInventory inv = Game.Player.Inventory;
        if (inv.TryMoveToInventory(item, true, false))
        {
            item.ItemLooted();
        }
    }

    private void LeftClickItem()
    {
        PlayerInventory inv = Game.Player.Inventory;
        var item = BackingItem;
        if (this.Type == InventoryItemButtonType.Ground)
        {
            if (inv.TryLootItemFromGround(BackingItem))
            {
                item.ItemLooted();
            }
        }
        else if (BackingItem.IsEquipment)
        {
            if (BackingItem == inv.GetEquipmentItem(BackingItem.Type))
            {
                if (inv.Unequip(BackingItem.Type))
                {
                    item.ItemLooted();
                    BackingItem = null;
                }
            }
            else
            {
                inv.Equip(BackingItem);
            }
        }
    }

    private void RightClickItem()
    {
        if (this.Type == InventoryItemButtonType.Inventory)
        {
            BackingItem.ItemDropped();
            Game.Player.Inventory.DiscardItem(BackingItem);
        }
    }

    public void ClearItem()
    {
        BackingItem = null;
        subImage.sprite = null;
        subImage.gameObject.SetActive(false);
        StackCount.gameObject.SetActive(false);
        DurabilitySlider.gameObject.SetActive(false);
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

            DurabilitySlider.gameObject.SetActive(false);
            if (item.ShowDurability)
            {
                DurabilitySlider.gameObject.SetActive(true);
                DurabilitySlider.transform.localScale = new Vector3(item.DurabilityRatio, 1.0f, 1.0f);
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

    Ground = 9,
}