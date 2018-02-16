using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : InventoryItem<WeaponCardData>
{
    public override InventoryItemType Type { get { return InventoryItemType.Weapon; } }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}