using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CursorObject : MonoBehaviour
{
    private void Update()
    {
        this.transform.position = Input.mousePosition;
    }

    public void SetImage(Sprite sprite)
    {
        var image = GetComponent<Image>();
        image.sprite = sprite;
    }
}

