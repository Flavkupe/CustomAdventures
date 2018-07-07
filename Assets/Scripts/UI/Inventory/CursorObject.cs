using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CursorObject : MonoBehaviour
{
    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    internal void SetImage(Sprite sprite, RectTransform rectTransform)
    {
        var image = GetComponent<Image>();
        image.sprite = sprite;
        image.rectTransform.sizeDelta = rectTransform.rect.size;
        transform.position = Input.mousePosition;
    }
}

