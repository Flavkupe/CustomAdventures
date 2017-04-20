using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    public CardMesh CardMesh { get; set; }

    public Rarity Rarity = Rarity.Basic;

    public abstract CardType CardType { get; }

    public Sprite CardArt;

	// Use this for initialization
	void Start ()
    {
        Debug.Assert(CardArt != null, "Be sure to set card art!"); 
	}

    public void InitCard()
    {
        // TODO: card based on rarity
        if (this.CardMesh == null)
        {
            this.CardMesh = Instantiate(DeckManager.Instance.CardMeshes.BasicCardMesh);
            this.CardMesh.transform.parent = this.transform;
            this.CardMesh.transform.position = new Vector3(0, 0, 0);
            this.CardMesh.SetCardArt(this.CardArt);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
    }
}

public enum CardType
{
    Dungeon,
    LevelUp,
    Loot,    
}

public enum Rarity
{
    Basic,
    Rare,
    Master
}