using TMPro;
using UnityEngine;

public class CardMesh : MonoBehaviourEx
{
    public SpriteRenderer CardArt;

    public TextMeshPro CardText;

    public TextMeshPro CardNameText;

    public void SetCardArt(Sprite sprite)
    {
        CardArt.sprite = sprite;
    }

    public void SetCardText(string text)
    {
        CardText.text = text;
    }

    public void SetCardName(string cardName)
    {
        CardNameText.text = cardName;
    }
}
