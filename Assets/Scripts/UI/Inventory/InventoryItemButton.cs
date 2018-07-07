using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class InventoryItemButton : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDropHandler, IEndDragHandler
{
    public InventoryItem BackingItem;

    public InventoryItemButtonType Type;

    public TextMeshProUGUI StackCount;

    public Image subImage;

    public Image DurabilitySlider;

    public bool IsOccupied => BackingItem != null;

    public bool IsEquipmentType => Type != InventoryItemButtonType.Inventory && Type != InventoryItemButtonType.Ground;

    private void Awake()
    {
        if (DurabilitySlider != null)
        {
            var pos = DurabilitySlider.rectTransform.localPosition;
            DurabilitySlider.rectTransform.localPosition = new Vector3(pos.x, -15.0f, pos.y);
        }

        //var trigger = GetComponent<EventTrigger>();        
        //var beginDrag = new EventTrigger.Entry();
        //var endDrag = new EventTrigger.Entry();
        //var drop = new EventTrigger.Entry();
        //beginDrag.eventID = EventTriggerType.BeginDrag;
        //endDrag.eventID = EventTriggerType.EndDrag;
        //drop.eventID = EventTriggerType.Drop;
        //beginDrag.callback.AddListener((data) => { OnStartDrag((PointerEventData)data); });        
        //endDrag.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });
        //drop.callback.AddListener((data) => { OnDrop((PointerEventData)data); });
        //trigger.triggers.Add(beginDrag);
        //trigger.triggers.Add(endDrag);
        //trigger.triggers.Add(drop);
    }

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
    private bool _successfullDrop = false;

    public void OnBeginDrag(PointerEventData data)
    {
        Debug.Log("Start Drag " + name);
        if (BackingItem != null)
        {
            _successfullDrop = false;
            _dragObject = Utils.InstantiateOfType<CursorObject>("drag");
            _dragObject.transform.SetParent(Game.UI.MainCanvas.transform);
            _dragObject.SetImage(BackingItem.ItemData.Sprite, subImage.rectTransform);
            subImage.gameObject.SetActive(false);
            
            // data.selectedObject = gameObject;
            Cursor.visible = false;
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        Debug.Log("End Drag " + name);
        Cursor.visible = true;
        if (_dragObject != null)
        {
            if (!_successfullDrop)
            {
                subImage.gameObject.SetActive(true);
            }

            Destroy(_dragObject.gameObject);
            _dragObject = null;
        }
    }

    public void OnDrop(PointerEventData data)
    {
        Debug.Log("On Drop " + name);
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
        if (!CanHoldItemType(otherItem))
        {
            // Wrong item type, so bail
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
            source.OnItemPlacedHere(BackingItem);
        }
        else
        {
            // Remove from other            
            source.OnItemRemovedFromHere();
        }

        OnItemPlacedHere(otherItem);
        source._successfullDrop = true;
        Game.UI.UpdateInventory();
    }

    protected virtual void OnItemRemovedFromHere()
    {
        if (BackingItem != null)
        {
            PlayerInventory inv = Game.Player.Inventory;
            inv.DestroyInventoryItem(BackingItem, false);
        }
    }

    protected virtual void OnItemPlacedHere(InventoryItem item)
    {
        PlayerInventory inv = Game.Player.Inventory;
        if (BackingItem != null)
        {
            OnItemRemovedFromHere();
        }

        if (inv.TryMoveToInventory(item, false, false))
        {
            item.PlayItemLootedSound();
        }
    }

    protected virtual bool CanHoldItemType(InventoryItem item)
    {
        return true;
    }

    private void LeftClickItem()
    {
        PlayerInventory inv = Game.Player.Inventory;
        var item = BackingItem;
        if (this.Type == InventoryItemButtonType.Ground)
        {
            if (inv.TryLootItemFromGround(BackingItem))
            {
                item.PlayItemLootedSound();
            }
        }
        else if (BackingItem.IsEquipment)
        {
            if (BackingItem == inv.GetEquipmentItem(BackingItem.Type))
            {
                if (inv.Unequip(BackingItem.Type))
                {
                    item.PlayItemLootedSound();
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
            BackingItem.PlayItemDroppedSound();
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