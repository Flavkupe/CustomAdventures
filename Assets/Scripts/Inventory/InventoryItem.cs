using System;
using UnityEngine;

public abstract class InventoryItem : StackableItem
{   
    public virtual InventoryItemType Type { get { return InventoryItemType.Misc; } }

    public abstract ItemCardData ItemData { get; }

    public abstract InventoryItem CloneInstance();

    public bool IsEquipment { get { return Type != InventoryItemType.Misc; } }   

    public TDataType GetData<TDataType>() where TDataType : ItemCardData
    {
        Debug.Assert(this.ItemData as TDataType, "Performing wrong data cast!");
        return this.ItemData as TDataType;
    }

    public override string Identifier { get { return ItemData.Name; } }
    public override int MaxStack { get { return ItemData.MaxStack; } }    

    public PassableTileItem AsPassableTileItem()
    {
        var obj = new GameObject(ItemData.Name);
        var component = obj.AddComponent<PassableTileItem>();
        component.Item = this;
        obj.transform.localScale *= this.ItemData.ScaleOnGround;
        return component;
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
    Misc = 1,    
    Weapon = 2,
    Armor = 3,
    Accessory = 4,
    Boots = 5,
    Shield = 6,
    Helmet = 7,
    Ring = 8,
}
