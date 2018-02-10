using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCard : LootCard<ItemCardData> {
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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
