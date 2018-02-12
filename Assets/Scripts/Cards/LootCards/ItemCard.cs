using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCard<T> : LootCard<T> where T : ItemCardData
{
    public override void ExecuteLootGetEvent()
    {
        InventoryItem item = Instantiate(this.Data.BackingItem);
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

    protected virtual InventoryItem CreateBackingItem()
    {
        GameObject obj = new GameObject();
        InventoryItem item = obj.AddComponent<InventoryItem>();
        return item;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
