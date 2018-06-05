using System;
using UnityEngine;

public abstract class InventoryItem : StackableItem
{   
    public virtual InventoryItemType Type { get { return InventoryItemType.Misc; } }

    public abstract ItemCardData ItemData { get; }

    public abstract InventoryItem CreateClone();

    public virtual bool ShowDurability { get { return false; } }

    public virtual float DurabilityRatio
    {
        get { return 1.0f; }
    }

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

    /// <summary>
    /// Such as using consumable or attacking with weapon
    /// </summary>
    public virtual void ItemUsed()
    {
    }

    public virtual int DefenseValue
    {
        get { return 0; }
    }

    public virtual void ItemEquipped()
    {
    }

    public virtual void ItemLooted()
    {
    }

    public virtual void ItemDropped()
    {
    }

    public virtual void ItemBroken()
    {
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

    public override InventoryItem CreateClone()
    {
        return new InventoryItem<TCardDataType>(_data);
    }

    public override InventoryItemType Type { get { return _data == null ? InventoryItemType.Misc : _data.ItemType; } }

    public TCardDataType Data { get { return _data; } }

    public override ItemCardData ItemData { get { return _data; } }

    public override void ItemLooted()
    {
        if (Data.CustomPickupSound != null)
        {
            Game.Sounds.PlayClip(Data.CustomPickupSound);
        }
        else
        {
            Game.Sounds.PlayFromClips(Game.Sounds.DefaultItemPickupSounds);
        }
    }

    public override void ItemDropped()
    {
        if (Data.CustomDropSound != null)
        {
            Game.Sounds.PlayClip(Data.CustomDropSound);
        }
        else
        {
            Game.Sounds.PlayFromClips(Game.Sounds.DefaultItemDropSounds);
        }
    }
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
