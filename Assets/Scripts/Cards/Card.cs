﻿using System;
using JetBrains.Annotations;
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
        Destroy(CardMesh.gameObject);
        Destroy(gameObject);
    }

    public virtual void SetData(CardData data)
    {
        Debug.Assert(data is TCardDataType, "Data must be of type " + typeof(TCardDataType));
        Data = data as TCardDataType;
        InitData();
    }

    [UsedImplicitly]
    private void Start ()
    {
        Debug.Assert(Data != null, "Must set Data!");
	}

    protected virtual void InitData()
    {
    }

    protected virtual CardMesh GetCardMesh()
    {
        switch (CardType) {
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
        transform.SetParent(Game.Decks.transform);

        // TODO: card based on rarity
        if (CardMesh == null)
        {
            CardMesh = Instantiate(GetCardMesh());
            CardMesh.transform.parent = transform;
            CardMesh.transform.position = new Vector3(0, 0, 0);
            CardMesh.SetCardArt(Data.CardArt);
            CardMesh.SetCardName(Data.Name);
            CardMesh.SetCardText(this.GetCardText());
        }
    }

    protected virtual string GetCardText()
    {
        return Data.CardText;
    }

    public void SetFaceUp()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void SetFaceDown()
    {
        transform.eulerAngles = new Vector3(0, 180, 0);
    }
}

public abstract class CardData : ScriptableObject
{
    public Rarity Rarity = Rarity.Basic;
    public Sprite CardArt;

    public string Name;

    [TextArea(3, 10)]
    public string CardText;

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