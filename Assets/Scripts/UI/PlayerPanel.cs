using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    public Text TextBox;

    public void UpdatePanel()
    {
        PlayerStats stats = Game.Player.Stats;
        string weaponName = stats.Inventory.EquippedWeapon == null ? "Fists" : stats.Inventory.EquippedWeapon.ItemData.Name;
        string data = string.Format(
@"Level: {0}
HP: {1}
Mulligans: {2}
Weapon: {3}
GameState: {4}
", stats.Level, stats.HP, stats.Mulligans, weaponName, Game.States.State.ToString());
        TextBox.text = data;
    }

    // Use this for initialization
    private void Awake ()
    {
        TextBox = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    private void Update ()
    {		
	}
}
