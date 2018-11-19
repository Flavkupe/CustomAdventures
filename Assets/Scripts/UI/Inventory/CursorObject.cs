using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CursorObject : MonoBehaviour
{
    private RectTransform rect;
    private void Update()
    {
        SetPosition();
    }

    internal void SetImage(Sprite sprite, RectTransform rectTransform)
    {
        var image = GetComponent<Image>();
        image.sprite = sprite;
        image.rectTransform.sizeDelta = rectTransform.rect.size;
        image.raycastTarget = false;
        SetPosition();
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void SetPosition()
    {
        transform.position = Utils.GetWorldMousePos();
        rect.anchoredPosition3D = rect.anchoredPosition3D.SetZ(0.0f);
    }
}

