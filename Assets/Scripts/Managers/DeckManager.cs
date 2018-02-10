using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckManager : SingletonObject<DeckManager>
{
    private DungeonCardData[] allDungeonCardData;

    public CardMeshes CardMeshes;

    public EnemyCard EnemyCardTemplate;
    public TreasureCard TreasureCardTemplate;

    public Deck<IDungeonCard> DungeonDeck = new Deck<IDungeonCard>();
    public Deck<ILootCard> LootDeck = new Deck<ILootCard>();
    public GameObject DungeonDeckHolder;
    public GameObject LootDeckHolder;

    public event EventHandler OnDrawAnimationDone;

    private void InitDecks()
    {
        // TODO TODO TODO left off here; make item decks now!
        this.allDungeonCardData = Resources.LoadAll<DungeonCardData>("Cards/DungeonCards");

        // Make dungeon deck
        CreateDeck<IDungeonCard>(10, DungeonDeck, DungeonDeckHolder, CreateDungeonCard);
    }

    private IDungeonCard CreateDungeonCard()
    {
        DungeonCardData data = this.allDungeonCardData.GetRandom();
        IDungeonCard card = null;
        switch (data.DungeonCardType)
        {
            case DungeonCardType.Treasure:
                card = Instantiate(TreasureCardTemplate);
                break;
            case DungeonCardType.Enemy:
            case DungeonCardType.Trap:
            default:
                card = Instantiate(EnemyCardTemplate);
                break;
        }

        card.SetData(data);
        return card;
    }

    private void CreateDeck<TCardType>(int numCards, Deck<TCardType> deck, GameObject deckHolder, Func<TCardType> createCardFunc) where TCardType : class, ICard
    {
        float yOffset = 0.0f;
        float zOffset = -0.001f;
        for (int i = 0; i < numCards; i++)
        {
            TCardType card = createCardFunc();
            deck.PushCard(card);
            card.InitCard();
            card.CardMesh.transform.position = deckHolder.transform.position;
            card.CardMesh.transform.parent = deckHolder.transform;
            card.CardMesh.SetFaceDown();
            card.CardMesh.transform.position = card.CardMesh.transform.position.IncrementBy(0, yOffset, zOffset);
            yOffset -= 0.05f;
            zOffset -= 0.001f;
        }
    }

    public List<IDungeonCard> DrawDungeonCards(int numDrawn)
    {
        List<IDungeonCard> cards = new List<IDungeonCard>();
        for (int i = 0; i < numDrawn; i++)
        {
            IDungeonCard card = DungeonDeck.DrawCard();
            cards.Add(card);
        }

        // Pause for animations
        GameManager.Instance.IsPaused = true;

        StartCoroutine(AnimateCardDraws(cards.Cast<ICard>().ToList()));
        return cards;
    }

    public IEnumerator AnimateCardDraws(List<ICard> cards)
    {
        int targetX = 0;

        foreach (ICard card in cards)
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