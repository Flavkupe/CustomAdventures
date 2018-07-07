using UnityEngine;

public abstract class InventoryItem : StackableItem
{   
    public virtual InventoryItemType Type => InventoryItemType.Misc;

    public abstract ItemCardData ItemData { get; }

    public abstract InventoryItem CreateClone();

    public virtual bool ShowDurability => false;

    public virtual float DurabilityRatio => 1.0f;

    public bool IsEquipment => Type != InventoryItemType.Misc;

    public TDataType GetData<TDataType>() where TDataType : ItemCardData
    {
        Debug.Assert(this.ItemData as TDataType, "Performing wrong data cast!");
        return this.ItemData as TDataType;
    }

    public override string Identifier => ItemData.Name;
    public override int MaxStack => ItemData.MaxStack;

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

    public virtual int DefenseValue => 0;

    public virtual void ItemEquipped()
    {
    }

    public virtual void ItemUnequipped()
    {
    }

    public virtual void PlayItemLootedSound()
    {
    }

    public virtual void PlayItemDroppedSound()
    {
    }

    public virtual void PlayItemBrokenSound()
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

    public override InventoryItemType Type => _data == null ? InventoryItemType.Misc : _data.ItemType;

    public TCardDataType Data => _data;

    public override ItemCardData ItemData => _data;

    public override void PlayItemLootedSound()
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

    public override void PlayItemDroppedSound()
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
