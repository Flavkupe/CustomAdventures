﻿using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    public void UpdatePanel()
    {
        var stats = Game.Player.GetPlayerStats();

// string weaponName = Game.Player.Inventory.EquippedWeapon == null ? "Fists" : Game.Player.Inventory.EquippedWeapon.ItemData.Name;

//        string data = string.Format(
//@"Level: {0}
//HP: {1}
//Mulligans: {2}
//Weapon: {3}
//Moves: {4}
//Actions: {5}
//", stats.Level, stats.HP, stats.Mulligans, weaponName, stats.FreeMoves, stats.FullActions);
//        TextBox.text = data;
    }

    // Use this for initialization
    private void Awake ()
    {
    }

    // Update is called once per frame
    private void Update ()
    {		
	}
}
