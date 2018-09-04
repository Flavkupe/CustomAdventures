using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CursorObject : MonoBehaviour
{
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

    private void SetPosition()
    {
        transform.position = Utils.GetWorldMousePos();
    }
}

