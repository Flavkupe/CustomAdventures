using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICard
{
    void InitCard();
    CardMesh CardMesh { get; set; }
    void SetData(CardData data);
    void DestroyCard();
}

public abstract class Card<TCardDataType> : MonoBehaviour, ICard where TCardDataType : CardData
{    
    public TCardDataType Data { get; protected set; }
    public abstract CardType CardType { get; }
    public CardMesh CardMesh { get; set; }

    public void DestroyCard()
    {
        Destroy(this.CardMesh.gameObject);
        Destroy(this.gameObject);
    }

    public virtual void SetData(CardData data)
    {
        Debug.Assert(data is TCardDataType, "Data must be of type " + typeof(TCardDataType));
        this.Data = data as TCardDataType;
    }

    // Use this for initialization
    void Start ()
    {
        Debug.Assert(this.Data != null, "Must set Data!");
	}

    public virtual void InitCard()
    {
        // TODO: card based on rarity
        if (this.CardMesh == null)
        {
            this.CardMesh = Instantiate(DeckManager.Instance.CardMeshes.BasicCardMesh);
            this.CardMesh.transform.parent = this.transform;
            this.CardMesh.transform.position = new Vector3(0, 0, 0);
            this.CardMesh.SetCardArt(this.Data.CardArt);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
    }
}

public abstract class CardData : ScriptableObject
{
    public Rarity Rarity = Rarity.Basic;
    public Sprite CardArt;
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