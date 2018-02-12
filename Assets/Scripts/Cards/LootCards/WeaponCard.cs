using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCard : ItemCard<WeaponCardData> {    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected override InventoryItem CreateBackingItem()
    {
        GameObject obj = new GameObject(Data.Name);
        Weapon weapon = obj.AddComponent<Weapon>();
        weapon.Data = Data;
        return weapon;
    }
}
