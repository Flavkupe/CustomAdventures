using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accessory : InventoryItem<ItemCardData>
{
    public override InventoryItemType Type { get { return InventoryItemType.Accessory; } }

    public override ItemCardData ItemData
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
