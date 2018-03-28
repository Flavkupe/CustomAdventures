using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Deck<T> where T : class, ICard
{
    private float yOffset;
    private float xOffset;
    private float zOffset = -0.001f;

    public GameObject DeckHolder;
    private Stack<T> deck = new Stack<T>();

    public int CardCount { get { return deck.Count; } }

    public void Init(IList<T> cards)
    {
        cards.Shuffle();
        RestackDeck(cards);
    }

    public void Shuffle()
    {
        List<T> cards = new List<T>(deck);
        cards.Shuffle();
        RestackDeck(cards);        
    }

    public void ShuffleCardInto(T card)
    {       
        List<T> cards = new List<T>(deck);
        cards.Add(card);
        cards.Shuffle();
        RestackDeck(cards);
    }

    public bool IsEmpty()
    {
        return deck.Count == 0;
    }

    public T DrawCard()
    {
        if (IsEmpty())
        {
            return null;
        }

        IncrementOffset();
        return deck.Pop();
    }

    public R DrawOfType<R>() where R : class, T
    {
        R card = deck.FirstOrDefault(a => a.GetType() == typeof(R)) as R;
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

            // Put items back in right order
            RestackDeck(newDeck.ToList());
        }
    }

    private void ResetOffset()
    {
        yOffset = 0.0f;
        xOffset = 0.0f;
        zOffset = 0.0f;
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

    private void RestackDeck(IList<T> cards)
    {
        deck.Clear();
        ResetOffset();
        PushCards(cards);
    }

    public void PushToBottom(IList<T> cards)
    {
        List<T> current = deck.ToList();
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
        deck.Push(card);
        var cardTransform = card.Object.transform;
        cardTransform.position = DeckHolder.transform.position;
        cardTransform.SetParent(DeckHolder.transform, true);
        cardTransform.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        card.SetFaceDown();
        cardTransform.localPosition = cardTransform.transform.localPosition.IncrementBy(xOffset, yOffset, zOffset);
        
        DecrementOffset();
    }

    public void ScaleDeck(float scaleMultiplier)
    {
        DeckHolder.transform.localScale *= scaleMultiplier;
    }
}
