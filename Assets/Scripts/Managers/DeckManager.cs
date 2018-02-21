using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckManager : SingletonObject<DeckManager>
{
    private List<DungeonCardData> allDungeonCardData;
    private List<LootCardData> allLootCardData;
    private List<CharacterCardData> allCharCardData;
    private List<AbilityCardData> allAbilityCardData;

    public CardMeshes CardMeshes;

    public Deck<IDungeonCard> DungeonDeck = new Deck<IDungeonCard>();
    public Deck<ILootCard> LootDeck = new Deck<ILootCard>();
    public Deck<ICharacterCard> CharacterDeck = new Deck<ICharacterCard>();
    public Deck<IAbilityCard> AbilityDeck = new Deck<IAbilityCard>();

    public GameObject DungeonDeckHolder;
    public GameObject LootDeckHolder;
    public GameObject CharDeckHolder;
    public GameObject AbilityDeckHolder;
    public GameObject CardDrawPos;

    public float DeckSmallSize = 0.35f;
    public float DeckBigSize = 1.00f;

    public event EventHandler OnDrawAnimationDone;

    private List<TCardDataType> LoadCards<TCardDataType>(string path) where TCardDataType : CardData
    {
        return Resources.LoadAll<TCardDataType>(path).Where(a => a.IncludeCard).ToList();
    }

    private void InitDecks()
    {
        allDungeonCardData = LoadCards<DungeonCardData>("Cards/DungeonCards");
        allLootCardData = LoadCards<LootCardData>("Cards/LootCards");
        allCharCardData = LoadCards<CharacterCardData>("Cards/CharacterCards");
        allAbilityCardData = LoadCards<AbilityCardData>("Cards/AbilityCards");

        // Make each deck
        CreateDeck(30, DungeonDeck, DungeonDeckHolder, allDungeonCardData);
        CreateDeck(30, LootDeck, LootDeckHolder, allLootCardData);
        CreateDeck(30, CharacterDeck, CharDeckHolder, allCharCardData);
        CreateDeck(10, AbilityDeck, AbilityDeckHolder, allAbilityCardData);
    }

    private void CreateDeck<TCardType, TCardDataType>(int numCards, Deck<TCardType> deck, GameObject deckHolder, 
        List<TCardDataType> cardData, Func<TCardDataType, TCardType> createCardFunc = null) where TCardType : class, ICard where TCardDataType : CardData
    {
        float yOffset = 0.0f;
        float xOffset = 0.0f;
        float zOffset = -0.001f;
        for (int i = 0; i < numCards; i++)
        {
            TCardDataType data = cardData.GetRandom();
            TCardType card = createCardFunc != null ? createCardFunc(data) : InstantiateOfType<TCardType>(data.BackingCardType);            
            card.SetData(data);
            deck.PushCard(card);
            card.InitCard();
            card.CardMesh.transform.position = deckHolder.transform.position;
            card.CardMesh.transform.SetParent(deckHolder.transform, true);
            card.CardMesh.SetFaceDown();
            card.CardMesh.transform.position = card.CardMesh.transform.position.IncrementBy(xOffset, yOffset, zOffset);
            yOffset -= 0.01f;
            xOffset -= 0.01f;
            zOffset -= 0.02f;
        }

        deckHolder.transform.localScale *= DeckSmallSize;
    }

    private List<TCardType> DrawCards<TCardType>(int numDrawn, Deck<TCardType> deck, GameObject deckHolder, float deckMoveSpeed = 10.0f) where TCardType : class, ICard
    {
        List<TCardType> cards = new List<TCardType>();
        for (int i = 0; i < numDrawn; i++)
        {
            TCardType card = deck.DrawCard();
            cards.Add(card);
        }

        // Pause for animations
        GameManager.Instance.IsPaused = true;

        StartCoroutine(AnimateCardDraws(cards.Cast<ICard>().ToList(), deckHolder));
        return cards;
    }

    public List<IDungeonCard> DrawDungeonCards(int numDrawn)
    {
        return this.DrawCards(numDrawn, DungeonDeck, DungeonDeckHolder);
    }

    public List<ILootCard> DrawLootCards(int numDrawn)
    {
        return this.DrawCards(numDrawn, LootDeck, LootDeckHolder);
    }

    public List<IAbilityCard> DrawAbilityCards(int numDrawn)
    {
        return this.DrawCards(numDrawn, AbilityDeck, AbilityDeckHolder);
    }

    public List<ICharacterCard> DrawCharacterCards(int numDrawn)
    {
        return this.DrawCards(numDrawn, CharacterDeck, CharDeckHolder, 30.0f);
    }

    public IEnumerator MoveDeckToPosition(GameObject deckHolder, Vector3 target, float sizeChange, float deckMoveSpeed = 10.0f)
    {        
        yield return StartCoroutine(deckHolder.MoveToSpotAndScaleCoroutine(target, deckMoveSpeed, sizeChange));
    }

    public IEnumerator AnimateCardDraws(List<ICard> cards, GameObject deckHolder, float deckMoveSpeed = 10.0f)
    {        
        float targetX = 0.0f;
        Vector3 initPos = deckHolder.transform.position;        
        yield return StartCoroutine(MoveDeckToPosition(deckHolder, CardDrawPos.transform.position, DeckBigSize - DeckSmallSize, deckMoveSpeed));

        foreach (ICard card in cards)
        {
            CardMesh cardMesh = card.CardMesh;
            targetX += 3.0f;
            Vector3 target = cardMesh.transform.position.IncrementBy(-targetX, 0.0f, 0.0f);
            yield return StartCoroutine(cardMesh.MoveToSpotCoroutine(target, 15.0f));
            yield return StartCoroutine(cardMesh.RotateCoroutine(Vector3.up, 180.0f, 200.0f));
            cardMesh.transform.eulerAngles = new Vector3(0.0f, 0.0f);
            cardMesh.transform.SetParent(null);
        }

        yield return new WaitForSeconds(1.0f);

        yield return StartCoroutine(MoveDeckToPosition(deckHolder, initPos, DeckSmallSize - DeckBigSize));

        yield return new WaitForSeconds(0.5f);

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
    public CardMesh DungeonBasicCardMesh;
    public CardMesh CharBasicCardMesh;
}