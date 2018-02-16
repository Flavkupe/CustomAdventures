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

public abstract class Card<TCardDataType> : MonoBehaviourEx, ICard where TCardDataType : CardData
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
        this.InitData();
    }

    // Use this for initialization
    void Start ()
    {
        Debug.Assert(this.Data != null, "Must set Data!");
	}

    protected virtual void InitData()
    {
    }

    protected virtual CardMesh GetCardMesh()
    {
        switch (this.CardType) {
            case CardType.Dungeon:
                return DeckManager.Instance.CardMeshes.DungeonBasicCardMesh;
            default:
                return DeckManager.Instance.CardMeshes.BasicCardMesh;
        }
    }

    public virtual void InitCard()
    {
        // TODO: card based on rarity
        if (this.CardMesh == null)
        {
            this.CardMesh = Instantiate(GetCardMesh());
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

    /// <summary>
    /// If this is false, this card will be excluded from all random deckbuilding
    /// </summary>
    public bool IncludeCard = true;

    public abstract Type BackingCardType { get; }
}

public enum CardType
{
    Dungeon,
    Character,
    Loot,
    Ability,
}

public enum Rarity
{
    Basic,
    Rare,
    Master
}