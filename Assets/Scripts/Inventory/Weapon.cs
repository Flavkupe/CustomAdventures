using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : InventoryItem
{
    public override InventoryItemType Type { get { return InventoryItemType.Weapon; } }

    public WeaponData Data;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

[Serializable]
public class WeaponData
{
    public int Power;
    public int Durability;
}