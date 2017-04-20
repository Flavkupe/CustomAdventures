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

    public void SetFaceUp()
    {
        this.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void SetFaceDown()
    {
        this.transform.eulerAngles = new Vector3(0, 180, 0);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
