using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

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

    public float CardMoveSpeed = 15.0f;

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
        List<TCardDataType> cardData) where TCardType : class, ICard where TCardDataType : CardData
    {
        deck.DeckHolder = deckHolder;
        for (int i = 0; i < numCards; i++)
        {
            TCardDataType data = cardData.GetRandom();
            TCardType card = CreateCardFromData<TCardType, TCardDataType>(data);
            deck.PushCard(card);
        }

        deck.ScaleDeck(DeckSmallSize);
    }

    public TCardInterface CreateAnonymousCardFromData<TCardInterface>(CardData data) where TCardInterface : class, ICard
    {
        var card = InstantiateOfType<TCardInterface>(data.BackingCardType, data.Name ?? data.name);
        if (card != null)
        {
            card.SetData(data);
            card.InitCard();
        }

        return card;
    }

    public TCardType CreateCardFromData<TCardType, TCardDataType>(TCardDataType data) where TCardType : class, ICard where TCardDataType : CardData
    {
        TCardType card = InstantiateOfType<TCardType>(data.BackingCardType, data.Name);
        if (card != null)
        {
            card.SetData(data);
            card.InitCard();
        }

        return card;
    }

    public List<TCardType> DrawCards<TCardType>(int numDrawn, Deck<TCardType> deck, Func<TCardType, bool> drawConditionFunc = null) where TCardType : class, ICard
    {
        List<TCardType> cards = new List<TCardType>();
        List<TCardType> invalidCards = new List<TCardType>();
        for (int i = 0; i < numDrawn; i++)
        {
            if (deck.IsEmpty())
            {
                break;
            }

            TCardType card = deck.DrawCard();

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

        deck.PushToBottom(invalidCards);

        return cards;
    }

    public IEnumerator MoveDeckToPosition(GameObject deckHolder, Vector3 target, float sizeChange, float deckMoveSpeed = 10.0f)
    {
        yield return deckHolder.transform.MoveToSpotAndScaleCoroutine(target, deckMoveSpeed, sizeChange);
    }

    public IEnumerator AnimateShuffleIntoDeck(ICard card, GameObject deckHolder, float deckMoveSpeed = 10.0f)
    {
        MonoBehaviourEx obj = card.Object;
        yield return obj.transform.MoveToSpotAndScaleCoroutine(deckHolder.transform.position, CardMoveSpeed, DeckSmallSize - DeckBigSize);
        yield return obj.RotateCoroutine(Vector3.up, 0.0f, 200.0f);
        obj.transform.eulerAngles = new Vector3(0.0f, 0.0f);
        obj.transform.SetParent(deckHolder.transform);        
    }

    private IEnumerator AnimateIndividualCardDraw(ICard card, float targetX)
    {
        MonoBehaviourEx obj = card.Object;        
        Vector3 target = obj.transform.position.IncrementBy(-targetX, 0.0f, 0.0f);
        yield return obj.transform.MoveToSpotCoroutine(target, CardMoveSpeed);
        yield return obj.RotateCoroutine(Vector3.up, 180.0f, 200.0f);
        obj.transform.eulerAngles = new Vector3(0.0f, 0.0f);
        obj.transform.SetParent(null);
    }

    public IEnumerator AnimateCardDraws(List<ICard> cards, GameObject deckHolder, float deckMoveSpeed = 10.0f)
    {
        float targetX = 0.0f;
        Vector3 initPos = deckHolder.transform.position;
        yield return MoveDeckToPosition(deckHolder, CardDrawPos.transform.position, DeckBigSize - DeckSmallSize, deckMoveSpeed);

        ParallelRoutineSet routineSet = new ParallelRoutineSet();
            
        foreach (ICard card in cards)
        {
            targetX += 3.0f;
            routineSet.AddRoutine(Routine.Create(AnimateIndividualCardDraw, card, targetX));
        }

        yield return routineSet;

        yield return new WaitForSecondsSpeedable(1.0f);

        yield return MoveDeckToPosition(deckHolder, initPos, DeckSmallSize - DeckBigSize);

        yield return new WaitForSecondsSpeedable(0.5f);

        Game.States.TriggerEvent(TriggeredEvent.CardDrawDone);
    }

    [UsedImplicitly]
    private void Awake()
    {
        Instance = this;
    }

    [UsedImplicitly]
    private void Start ()
    {
        InitDecks();
    }
}


[Serializable]
public class CardMeshes
{
    public CardMesh BasicCardMesh;
    public CardMesh DungeonBasicCardMesh;
    public CardMesh CharBasicCardMesh;
    public CardMesh AbilityBasicCardMesh;
}