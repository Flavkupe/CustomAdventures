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

        void Awake()
        {
            // this._rect = 
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_tooltip == null)
            {
                _tooltip = Instantiate(Game.UI.Templates.UIParts.HoverTooltip);
                _tooltip.transform.SetParent(this.transform);
                _tooltip.transform.localScale = Vector3.one;
                _tooltip.transform.position = Utils.GetWorldMousePos();
                _tooltip.SetText(TextValue);
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
