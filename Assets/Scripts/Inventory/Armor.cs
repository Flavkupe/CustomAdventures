using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : InventoryItem
{
    public override InventoryItemType Type { get { return InventoryItemType.Armor; } }

    public override ItemCardData BaseItemData
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
