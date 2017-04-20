using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckManager : SingletonObject<DeckManager>
{
    private DungeonCard[] allDungeonCards;

    public CardMeshes CardMeshes;

    public Deck<DungeonCard> DungeonDeck = new Deck<DungeonCard>();
    public GameObject DungeonDeckHolder;

    public event EventHandler OnDrawAnimationDone;

    private void InitDecks()
    {
        float yOffset = 0.0f;
        float zOffset = -0.001f;
        this.allDungeonCards = Resources.LoadAll<DungeonCard>("Cards/DungeonCards");
        for (int i = 0; i < 10; i++)
        {
            DungeonCard card = Instantiate(this.allDungeonCards.GetRandom());
            this.DungeonDeck.PushCard(card);
            card.InitCard();
            card.CardMesh.transform.position = this.DungeonDeckHolder.transform.position;
            card.CardMesh.transform.parent = this.DungeonDeckHolder.transform;
            card.CardMesh.SetFaceDown();
            card.CardMesh.transform.position = card.CardMesh.transform.position.IncrementBy(0, yOffset, zOffset);
            yOffset -= 0.05f;
            zOffset -= 0.001f;
        }
    }

    public List<DungeonCard> DrawDungeonCards(int numDrawn)
    {
        List<DungeonCard> cards = new List<DungeonCard>();
        for (int i = 0; i < numDrawn; i++)
        {
            DungeonCard card = DungeonDeck.DrawCard();
            cards.Add(card);
        }

        // Pause for animations
        GameManager.Instance.IsPaused = true;

        StartCoroutine(AnimateCardDraws(cards.Cast<Card>().ToList()));
        return cards;
    }

    public IEnumerator AnimateCardDraws(List<Card> cards)
    {
        int targetX = 0;

        foreach (Card card in cards)
        {
            CardMesh cardMesh = card.CardMesh;                        
            targetX += 3;
            Vector3 target = cardMesh.transform.position.IncrementBy(-targetX, 0.0f, 0.0f);
            yield return StartCoroutine(cardMesh.MoveToSpotCoroutine(target, 15.0f));
            yield return StartCoroutine(cardMesh.RotateCoroutine(Vector3.up, 180.0f, 200.0f));
            cardMesh.transform.eulerAngles = new Vector3(0.0f, 0.0f);
        }

        yield return new WaitForSeconds(1.0f);

        if (OnDrawAnimationDone != null)
        {
            OnDrawAnimationDone.Invoke(this, new EventArgs());
        }
    }

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start ()
    {        
        InitDecks();
    }
	
	// Update is called once per frame
	void Update ()
    {		
	}
}

[Serializable]
public class CardMeshes
{
    public CardMesh BasicCardMesh;
}