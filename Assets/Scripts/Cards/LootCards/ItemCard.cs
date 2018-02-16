using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemCard<TItemCardDataType> : LootCard<TItemCardDataType> where TItemCardDataType : ItemCardData
{
    public override void ExecuteLootGetEvent()
    {
        InventoryItem item = this.Data.BackingItem.CloneInstance();
        if (Player.Instance.TryMoveToInventory(item, true))
        {
            // TODO: message?
        }
        else
        {
            // TODO: full inventory
        }
    }

    protected override void InitData()
    {
        base.InitData();
        this.Data.BackingItem = this.CreateBackingItem();
    }

    protected abstract InventoryItem<TItemCardDataType> AddBackingComponent(GameObject obj);

    protected virtual InventoryItem CreateBackingItem()
    {
        GameObject obj = new GameObject();
        InventoryItem<TItemCardDataType> item = this.AddBackingComponent(obj);
        item.SetData(this.Data);
        return item;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
