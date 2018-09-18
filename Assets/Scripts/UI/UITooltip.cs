using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UITooltip : MonoBehaviour
    {
        public TextMeshProUGUI Text;
        public RectTransform ContentsRect;

        private RectTransform _tooltipRect;

        private void Awake()
        {
            _tooltipRect = GetComponent<RectTransform>();
        }

        public void SetText(string text)
        {
            Text.text = text;
            var textSize = Text.GetPreferredValues();
            textSize += new Vector2(40.0f, 20.0f); // Padding
            _tooltipRect.sizeDelta = new Vector2(textSize.x * ContentsRect.localScale.x, textSize.y * ContentsRect.localScale.y);
            ContentsRect.sizeDelta = textSize;
        }
    }
}
