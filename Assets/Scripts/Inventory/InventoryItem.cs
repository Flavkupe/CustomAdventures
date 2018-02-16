using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem : MonoBehaviour
{
    public virtual InventoryItemType Type { get { return InventoryItemType.Misc; } }

    public abstract ItemCardData ItemData { get; }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    public abstract InventoryItem CloneInstance();
}

public class InventoryItem<TCardDataType> : InventoryItem where TCardDataType: ItemCardData
{
    private TCardDataType data;

    public void SetData(TCardDataType data)
    {
        this.data = data;
    }

    public override InventoryItem CloneInstance()
    {
        var clone = Instantiate(this);
        clone.SetData(this.data);
        return clone;
    }

    public TCardDataType Data { get { return data; } }

    public override ItemCardData ItemData { get { return data; } }
}

public enum InventoryItemType
{
    Misc,
    Weapon,
    Armor,
    Accessory,
}
