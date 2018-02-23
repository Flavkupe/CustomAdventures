using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem
{
    public int CurrentStackSize = 1;

    public virtual InventoryItemType Type { get { return InventoryItemType.Misc; } }

    public abstract ItemCardData ItemData { get; }

    public abstract InventoryItem CloneInstance();

    public virtual bool ItemCanStack(InventoryItem other)
    {
        return other.ItemData.Name == this.ItemData.Name;
    }

    /// <summary>
    /// Adds other item to stack, changing CurrentStackSize for each
    /// </summary>
    public virtual void StackItems(InventoryItem other)
    {
        int spaceLeft = this.ItemData.MaxStack - this.CurrentStackSize;
        int newItems = Math.Min(spaceLeft, other.CurrentStackSize);
        int leftOver = other.CurrentStackSize - newItems;
        this.CurrentStackSize += newItems;
        other.CurrentStackSize = leftOver;
    }
}

public class InventoryItem<TCardDataType> : InventoryItem where TCardDataType : ItemCardData
{
    private TCardDataType data;

    public InventoryItem(TCardDataType data) 
    {
        this.data = data;
    }

    public void SetData(TCardDataType data)
    {
        this.data = data;
    }

    public override InventoryItem CloneInstance()
    {
        return new InventoryItem<TCardDataType>(this.data);
    }

    public override InventoryItemType Type { get { return data == null ? InventoryItemType.Misc : data.ItemType; } }

    public TCardDataType Data { get { return data; } }

    public override ItemCardData ItemData { get { return data; } }
}

public enum InventoryItemType
{
    Misc,
    Weapon,
    Armor,
    Accessory,
}
