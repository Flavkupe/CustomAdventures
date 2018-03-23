﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICard
{
    void InitCard();
    CardMesh CardMesh { get; set; }
    void SetData(CardData data);
    void DestroyCard();
    MonoBehaviourEx Object { get; }

    void SetFaceUp();
    void SetFaceDown();
}

public abstract class Card<TCardDataType> : MonoBehaviourEx, ICard where TCardDataType : CardData
{    
    public TCardDataType Data { get; protected set; }
    public abstract CardType CardType { get; }
    public CardMesh CardMesh { get; set; }

    public MonoBehaviourEx Object { get { return this; } }

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
            case CardType.Character:
                return Game.Decks.CardMeshes.CharBasicCardMesh;
            case CardType.Dungeon:
                return Game.Decks.CardMeshes.DungeonBasicCardMesh;
            case CardType.Ability:
                return Game.Decks.CardMeshes.AbilityBasicCardMesh;
            default:
                return Game.Decks.CardMeshes.BasicCardMesh;
        }
    }

    public virtual void InitCard()
    {
        this.transform.SetParent(Game.Decks.transform);

        // TODO: card based on rarity
        if (this.CardMesh == null)
        {
            this.CardMesh = Instantiate(GetCardMesh());
            this.CardMesh.transform.parent = this.transform;
            this.CardMesh.transform.position = new Vector3(0, 0, 0);
            this.CardMesh.SetCardArt(this.Data.CardArt);
        }
    }

    public void SetFaceUp()
    {
        this.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void SetFaceDown()
    {
        this.transform.eulerAngles = new Vector3(0, 180, 0);
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

    public string Name;

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