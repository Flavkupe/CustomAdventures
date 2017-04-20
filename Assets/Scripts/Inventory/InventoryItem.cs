using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public virtual InventoryItemType Type { get { return InventoryItemType.Misc; } } 

    public Sprite Icon;

    public string Name;

    public int GoldValue = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public enum InventoryItemType
{
    Weapon,
    Misc,
}
