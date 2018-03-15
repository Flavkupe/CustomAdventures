using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardMesh : MonoBehaviourEx
{
    public SpriteRenderer CardArt;

    public TextMeshPro CardText;

    public void SetCardArt(Sprite sprite)
    {
        this.CardArt.sprite = sprite;
    }

    public void SetCardText(string text)
    {
        this.CardText.text = text;
    }

    // Use this for initialization
    void Start () {
	}

	// Update is called once per frame
	void Update () {
	}
}
