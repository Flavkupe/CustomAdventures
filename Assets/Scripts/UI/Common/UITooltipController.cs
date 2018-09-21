using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UITooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private UITooltip _tooltip;
        private RectTransform _rect;

        public string TextValue;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_tooltip == null)
            {
                _tooltip = Instantiate(Game.UI.Templates.UIParts.HoverTooltip);
                _tooltip.transform.SetParent(this.transform);
                _tooltip.transform.localScale = Vector3.one;
                var replaced = Game.Tokens.ReplaceTokens(TextValue);
                _tooltip.SetText(replaced);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_tooltip != null)
            {
                Destroy(_tooltip.gameObject);
                _tooltip = null;
            }
        }
    }
}
