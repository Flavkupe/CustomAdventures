using TMPro;
using UnityEngine;

public class CardMesh : MonoBehaviourEx
{
    public SpriteRenderer CardArt;

    public TextMeshPro CardText;

    public void SetCardArt(Sprite sprite)
    {
        CardArt.sprite = sprite;
    }

    public void SetCardText(string text)
    {
        CardText.text = text;
    }
}
