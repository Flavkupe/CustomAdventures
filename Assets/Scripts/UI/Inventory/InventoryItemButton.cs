using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public abstract class InventoryItemButton : MonoBehaviour, IBeginDragHandler, IDropHandler, IEndDragHandler
{
    public InventoryItem BackingItem;

    public InventoryItemButtonType Type;

    public TextMeshProUGUI StackCount;

    public Image subImage;

    public Image DurabilitySlider;

    public bool IsOccupied => BackingItem != null;

    public virtual bool IsGroundItem => false;

    public virtual bool IsEquipmentType => false;

    private void Awake()
    {
        if (DurabilitySlider != null)
        {
            var pos = DurabilitySlider.rectTransform.localPosition;
            DurabilitySlider.rectTransform.localPosition = new Vector3(pos.x, -15.0f, pos.y);
        }
    }

    private CursorObject _dragObject;

    public void OnBeginDrag(PointerEventData data)
    {
        if (BackingItem != null)
        {
            _dragObject = Utils.InstantiateOfType<CursorObject>("drag");
            _dragObject.transform.SetParent(Game.UI.MainCanvas.transform);
            _dragObject.SetImage(BackingItem.ItemData.Sprite, subImage.rectTransform);
            _dragObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            subImage.gameObject.SetActive(false);
            Cursor.visible = false;
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        FinishDragEffects();
    }

    private void FinishDragEffects()
    {
        Cursor.visible = true;
        if (_dragObject != null)
        {
            if (BackingItem != null)
            {
                subImage.gameObject.SetActive(true);
            }

            Destroy(_dragObject.gameObject);
            _dragObject = null;
        }
    }

    public void OnDrop(PointerEventData data)
    {
        if (data.pointerDrag != null)
        {
            var button = data.pointerDrag.GetComponent<InventoryItemButton>();
            if (button != null)
            {
                DropItemHere(button);
            }
        }
    }

    private void DropItemHere(InventoryItemButton source)
    {
        if (source == null || source.BackingItem == null)
        {
            return;
        }

        var otherItem = source.BackingItem;
        if (!CanAcceptFromSource(source) || !CanHoldItemType(otherItem))
        {
            // Wrong source or item type, so bail
            return;
        }

        // If this slot has an item, try to swap with source.
        // Otherwise clear the source
        if (BackingItem != null)
        {
            if (!source.CanHoldItemType(BackingItem))
            {
                // other cannot hold this item, so bail
                return;
            }

            // Swap with other
            var toSwap = BackingItem;
            OnItemRemovedFromHere();
            OnItemPlacedHere(otherItem);
            source.OnItemRemovedFromHere();
            source.OnItemPlacedHere(toSwap);
        }
        else
        {
            // Remove from other            
            source.OnItemRemovedFromHere();
            OnItemPlacedHere(otherItem);
        }
        
        Game.UI.UpdateInventory();
    }

    protected virtual bool CanAcceptFromSource(InventoryItemButton source)
    {
        return source.Type != this.Type;
    }

    protected abstract void OnItemRemovedFromHere();

    protected abstract void OnItemPlacedHere(InventoryItem item);

    protected abstract bool CanHoldItemType(InventoryItem item);

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

    public void DestroyButton()
    {
        FinishDragEffects();
        Destroy(gameObject, 0.1f);
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