using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Deck<T> where T : Card
{
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
        List<T> cards = new List<T>(deck);
        cards.Shuffle();
        deck.Clear();
        cards.ForEach(a => deck.Push(a));
    }

    public void ShuffleCardInto(T card)
    {
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

        return deck.Pop();
    }

    public void PushCard(T card)
    {
        this.deck.Push(card);
    }
}
