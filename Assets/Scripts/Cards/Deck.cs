using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Deck<T> where T : class, ICard
{
    private float _yOffset;
    private float _xOffset;
    private float _zOffset = -0.001f;

    public GameObject DeckHolder;
    private readonly Stack<T> _deck = new Stack<T>();

    public int CardCount => _deck.Count;

    public void Init(IList<T> cards)
    {
        cards.Shuffle();
        RestackDeck(cards);
    }

    public void Shuffle()
    {
        List<T> cards = new List<T>(_deck);
        cards.Shuffle();
        RestackDeck(cards);        
    }

    public void ShuffleCardInto(T card)
    {       
        List<T> cards = new List<T>(_deck);
        cards.Add(card);
        cards.Shuffle();
        RestackDeck(cards);
    }

    public bool IsEmpty()
    {
        return _deck.Count == 0;
    }

    public T DrawCard()
    {
        if (IsEmpty())
        {
            return null;
        }

        IncrementOffset();
        return _deck.Pop();
    }

    public TR DrawOfType<TR>() where TR : class, T
    {
        TR card = _deck.FirstOrDefault(a => a.GetType() == typeof(TR)) as TR;
        RemoveCard(card);
        return card;
    }

    public void RemoveCard(T card)
    {
        // TODO: change mesh order
        if (card != null)
        {
            Stack<T> newDeck = new Stack<T>();
            foreach (T item in _deck)
            {
                if (item != card)
                {
                    newDeck.Push(item);
                }
            }

            // Put items back in right order
            RestackDeck(newDeck.ToList());
        }
    }

    private void ResetOffset()
    {
        _yOffset = 0.0f;
        _xOffset = 0.0f;
        _zOffset = 0.0f;
    }

    private void IncrementOffset()
    {
        _yOffset += 0.01f;
        _xOffset += 0.01f;
        _zOffset += 0.02f;
    }

    private void DecrementOffset()
    {
        _yOffset -= 0.01f;
        _xOffset -= 0.01f;
        _zOffset -= 0.02f;
    }

    private void RestackDeck(IList<T> cards)
    {
        _deck.Clear();
        ResetOffset();
        PushCards(cards);
    }

    public void PushToBottom(IList<T> cards)
    {
        List<T> current = _deck.ToList();
        current.AddRange(cards);
        RestackDeck(current);
    }

    public void PushCards(IList<T> cards)
    {
        // Push cards in the "correct" order (ie starting from back
        // of list to preserve list order)
        for(int i = cards.Count - 1; i >= 0; --i)
        {
            T card = cards[i];
            PushCard(card);
        }
    }

    public void PushCard(T card)
    {
        _deck.Push(card);
        var cardTransform = card.Object.transform;
        cardTransform.position = DeckHolder.transform.position;
        cardTransform.SetParent(DeckHolder.transform, true);
        cardTransform.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        card.SetFaceDown();
        cardTransform.localPosition = cardTransform.transform.localPosition.IncrementBy(_xOffset, _yOffset, _zOffset);
        
        DecrementOffset();
    }

    public List<T> DrawCards(int numDrawn, Func<T, bool> drawConditionFunc = null)
    {
        var cards = new List<T>();
        var invalidCards = new List<T>();
        for (var i = 0; i < numDrawn; i++)
        {
            if (IsEmpty())
            {
                break;
            }

            var card = DrawCard();
            if (drawConditionFunc != null && !drawConditionFunc(card))
            {
                i--; // don't count the draw
                invalidCards.Add(card);
            }
            else
            {
                cards.Add(card);
            }
        }

        PushToBottom(invalidCards);

        return cards;
    }

    public void ScaleDeck(float scaleMultiplier)
    {
        DeckHolder.transform.localScale *= scaleMultiplier;
    }
}
