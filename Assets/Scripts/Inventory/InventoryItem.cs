using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem : MonoBehaviour
{
    public virtual InventoryItemType Type { get { return InventoryItemType.Misc; } }

    public abstract ItemCardData BaseItemData { get; }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public enum InventoryItemType
{
    Misc,
    Weapon,
    Armor,
    Accessory,
}
