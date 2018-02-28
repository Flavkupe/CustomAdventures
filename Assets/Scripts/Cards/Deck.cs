using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Deck<T> where T : class, ICard
{
    private float yOffset = 0.0f;
    private float xOffset = 0.0f;
    private float zOffset = -0.001f;

    public GameObject DeckHolder;

    Stack<T> deck = new Stack<T>();

    public void Init(IList<T> cards)
    {
        cards.Shuffle();
        foreach (T card in cards)
        {
            deck.Push(card);
        }
    }    

    public void Shuffle()
    {
        // TODO: change mesh order
        List<T> cards = new List<T>(deck);
        cards.Shuffle();
        deck.Clear();
        cards.ForEach(a => deck.Push(a));
    }

    public void ShuffleCardInto(T card)
    {
        // TODO: change mesh order
        List<T> cards = new List<T>(deck);
        cards.Add(card);
        cards.Shuffle();
        deck.Clear();
        cards.ForEach(a => deck.Push(a));
    }

    public T DrawCard()
    {
        if (deck.Count == 0)
        {
            return null;
        }

        IncrementOffset();
        return deck.Pop();
    }

    public R DrawOfType<R>() where R : class, T
    {
        R card = this.deck.FirstOrDefault(a => a.GetType() == typeof(R)) as R;
        RemoveCard(card);
        return card;
    }

    public void RemoveCard(T card)
    {
        // TODO: change mesh order
        if (card != null)
        {
            Stack<T> newDeck = new Stack<T>();
            foreach (T item in deck)
            {
                if (item != card)
                {
                    newDeck.Push(item);
                }
            }

            deck.Clear();
            foreach (T item in newDeck)
            {
                // Put items back in right order
                deck.Push(item);
            }
        }
    }

    private void IncrementOffset()
    {
        yOffset += 0.01f;
        xOffset += 0.01f;
        zOffset += 0.02f;
    }

    private void DecrementOffset()
    {
        yOffset -= 0.01f;
        xOffset -= 0.01f;
        zOffset -= 0.02f;
    }

    public void PushCard(T card)
    {
        this.deck.Push(card);
        card.CardMesh.transform.position = DeckHolder.transform.position;
        card.CardMesh.transform.SetParent(DeckHolder.transform, true);
        card.CardMesh.transform.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        card.CardMesh.SetFaceDown();
        card.CardMesh.transform.position = card.CardMesh.transform.position.IncrementBy(xOffset, yOffset, zOffset);
        
        DecrementOffset();
    }

    public void ScaleDeck(float scaleMultiplier)
    {
        DeckHolder.transform.localScale *= scaleMultiplier;
    }
}
