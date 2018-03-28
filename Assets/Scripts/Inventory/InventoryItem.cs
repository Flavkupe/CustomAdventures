using System;

public abstract class InventoryItem
{
    public int CurrentStackSize = 1;

    public virtual InventoryItemType Type { get { return InventoryItemType.Misc; } }

    public abstract ItemCardData ItemData { get; }

    public abstract InventoryItem CloneInstance();

    public virtual bool ItemCanStack(InventoryItem other)
    {
        return other.ItemData.Name == ItemData.Name;
    }

    /// <summary>
    /// Adds other item to stack, changing CurrentStackSize for each
    /// </summary>
    public virtual void StackItems(InventoryItem other)
    {
        int spaceLeft = ItemData.MaxStack - CurrentStackSize;
        int newItems = Math.Min(spaceLeft, other.CurrentStackSize);
        int leftOver = other.CurrentStackSize - newItems;
        CurrentStackSize += newItems;
        other.CurrentStackSize = leftOver;
    }
}

public class InventoryItem<TCardDataType> : InventoryItem where TCardDataType : ItemCardData
{
    private TCardDataType _data;

    public InventoryItem(TCardDataType data) 
    {
        _data = data;
    }

    public void SetData(TCardDataType data)
    {
        _data = data;
    }

    public override InventoryItem CloneInstance()
    {
        return new InventoryItem<TCardDataType>(_data);
    }

    public override InventoryItemType Type { get { return _data == null ? InventoryItemType.Misc : _data.ItemType; } }

    public TCardDataType Data { get { return _data; } }

    public override ItemCardData ItemData { get { return _data; } }
}

public enum InventoryItemType
{
    Misc,
    Weapon,
    Armor,
    Accessory,
}
