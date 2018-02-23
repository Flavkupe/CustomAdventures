using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    public Text TextBox;

    public void UpdatePanel()
    {
        PlayerStats stats = Player.Instance.Stats;
        string weaponName = stats.Inventory.EquippedWeapon == null ? "Fists" : stats.Inventory.EquippedWeapon.ItemData.Name;
        string data = string.Format(
@"Level: {0}
HP: {1}
Energy: {2}
Weapon: {3}
GameState: {4}
", stats.Level, stats.HP, stats.Energy, weaponName, GameManager.Instance.State.ToString());
        this.TextBox.text = data;
    }

	// Use this for initialization
	void Awake ()
    {
        this.TextBox = this.GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update ()
    {		
	}
}
