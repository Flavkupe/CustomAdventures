using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCard : ItemCard<WeaponCardData> {

    protected override InventoryItem<WeaponCardData> AddBackingComponent(GameObject obj)
    {
        return obj.AddComponent<Weapon>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
}
