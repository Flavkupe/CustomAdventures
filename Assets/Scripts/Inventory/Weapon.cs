using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : InventoryItem
{
    public override InventoryItemType Type { get { return InventoryItemType.Weapon; } }

    public override ItemCardData BaseItemData { get { return Data; } }

    public WeaponCardData Data;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}