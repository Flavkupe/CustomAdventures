using UnityEngine;
using UnityEngine.UI;

public class EntityPanel : MonoBehaviour
{
    public Text TextBox;

    // Use this for initialization
    private void Start ()
    {
        TextBox = GetComponentInChildren<Text>();
    }

    public void ShowEnemyData(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        EnemyCardData data = enemy.Data;
        if (data == null)
        {
            return;
        }

        TextBox.text = string.Format(
@"Name: {0}
Level: {1}
HP: {2}
EXP: {3}
", data.Name, data.Level, data.MaxHP, data.EXP);
    }
}
