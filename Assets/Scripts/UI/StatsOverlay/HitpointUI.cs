using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HitpointUI : MonoBehaviour {

    public TextMeshProUGUI HpText;

    public RectTransform HpRect;

    private int _currentHP;

    private RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update ()
    {
        var player = Game.Player;
        if (Input.GetKeyUp(KeyCode.L))
        {
            player.CurrentStats.HP--;
        }

        if (player!= null) {
            if (_currentHP != player.CurrentStats.HP && player.BaseStats.HP > 0)
            {
                float ratio = (float)player.CurrentStats.HP / player.BaseStats.HP;
                _currentHP = player.CurrentStats.HP;
                HpText.text = _currentHP.ToString();
                var height = rect.rect.height * (1.0f - ratio);
                HpRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }
    }
}
