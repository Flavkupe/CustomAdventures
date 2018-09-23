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
        if (player!= null) {
            if (player.CurrentStats.HP != _currentHP && player.BaseStats.HP > 0)
            {
                float ratio = (float)player.CurrentStats.HP.Value / player.BaseStats.HP.Value;
                _currentHP = player.CurrentStats.HP.Value;
                HpText.text = _currentHP.ToString();
                var height = rect.rect.height * (1.0f - ratio);
                HpRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }
    }
}
