﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CardAnimationController))]
public class CardEventController : MonoBehaviourEx
{
    public string TakeKey = " ";
    public string MulliganKey = "q";
    public AudioClip MulliganSound;

    private DeckManager _decks;

    private CardAnimationController _animationController;

    private Dungeon _dungeon;

    private void Awake()
    {
        _animationController = GetComponent<CardAnimationController>();
        _dungeon = GetComponentInParent<Dungeon>();        
    }

    private void Start()
    {
        // TODO
        _decks = Game.Decks;
    }

    public IEnumerator PerformCharacterCardEvents(Player player)
    {
        // TODO: replace 2 with something
        var props = new DrawCoroutineProps<ICharacterCard>(2, _decks.CharacterDeck, true);
        yield return DrawCardsWithMulligan(props);

        var context = new CharacterCardExecutionContext(player, _decks);
        // Create parallel set of routines that run in parallel
        var set = ParallelRoutineSet.CreateSet(props.DrawResults, card => Routine.Create(card.ApplyEffect, context));
        yield return set;

        DestroyCards(props.DrawResults);
    }

    public IEnumerator PerformAbilityCardEvents(Player player)
    {
        var props = new DrawCoroutineProps<IAbilityCard>(1, _decks.AbilityDeck, false);
        yield return QuickDrawCoroutine(props);

        foreach (var card in props.DrawResults)
        {
            player.EquipDrawnAbilityCard(card);
        }
    }

    public IEnumerator PerformLootEvents(LootEventProperties lootProperties)
    {
        Func<ILootCard, bool> drawConditionFunc = null;
        var filter = lootProperties.CreateLootCardFilter();
        if (filter != null && filter.PossibleTypes.Count > 0)
        {
            drawConditionFunc = card => filter.PossibleTypes.Contains(card.LootCardType);
        }

        var draws = lootProperties.NumTreasures;
        var props = new DrawCoroutineProps<ILootCard>(draws, _decks.LootDeck, true, drawConditionFunc);
        yield return DrawCardsWithMulligan(props);

        // TODO: find player
        var context = new LootCardExecutionContext(Game.Player);

        // Create parallel set of routines that run in parallel
        var set = ParallelRoutineSet.CreateSet(props.DrawResults, card => Routine.Create(card.ExecuteLootEvent, context));
        yield return set;

        DestroyCards(props.DrawResults);
    }

    public IEnumerator PerformDungeonEvents(RoomArea roomArea)
    {
        var draws = roomArea.NumDraws;
        DungeonCardData[] specialCards = null;
        if (roomArea.IsBossArea)
        {
            specialCards = _dungeon.PossibleBossCards;
        }
        else if (roomArea.IsEntranceArea)
        {
            specialCards = Game.Player.GetEntranceCards();
        }

        if (specialCards != null)
        {
            foreach (var cardData in specialCards)
            {
                var specialCard = cardData.CreateCard<IDungeonCard>();
                _decks.DungeonDeck.PushCard(specialCard);
            }

            draws = specialCards.Length;
        }

        roomArea.gameObject.SetActive(false);
        _dungeon.PostGroupEventCleanup();

        var props = new DrawCoroutineProps<IDungeonCard>(draws, _decks.DungeonDeck, roomArea.CanMulligan);
        yield return DrawCardsWithMulligan(props);

        var context = new DungeonCardExecutionContext(_dungeon, Game.Player, roomArea);

        // Create parallel set of routines that run in parallel
        var set = ParallelRoutineSet.CreateSet(props.DrawResults, card => Routine.Create(card.ExecuteDungeonEvent, context));
        yield return set;

        DestroyCards(props.DrawResults);
    }

    private void DestroyCards(IEnumerable<ICard> cards)
    {
        foreach (var card in cards)
        {
            card.DestroyCard();
        }
    }

    private IEnumerator QuickDrawCoroutine<TCardType>(DrawCoroutineProps<TCardType> props) where TCardType : class, ICard
    {
        List<TCardType> cards = props.Deck.DrawCards(props.NumDraws);
        foreach (var card in cards)
        {
            yield return _animationController.AnimateQuickSlideupDraw(card);
        }

        props.DrawResults.AddRange(cards);
    }

    private IEnumerator DrawCardsWithMulligan<TCardType>(DrawCoroutineProps<TCardType> props)
        where TCardType : class, ICard
    {
        _dungeon.PauseActions();

        // TODO: replace this state system
        yield return Game.States.WaitUntilNotState(GameState.CharacterMoving);

        List<TCardType> cards;
        while (true)
        {
            cards = props.Deck.DrawCards(props.NumDraws, props.DrawConditionFunc).ToList();
            yield return _animationController.AnimateCardDraws(cards, props.Deck.DeckHolder);
            if (props.AllowMulligan && Game.Player.GetPlayerStats().Mulligans > 0)
            {
                Game.UI.ToggleMulliganPanel(true);
                var pressEvent = new AwaitKeyPress(MulliganKey, TakeKey);
                var triggerEvent = new AwaitTriggerEvent<UIEvent>(Game.UI.GetCurrentUIEvent, UIEvent.MulliganPressed, UIEvent.TakePressed);
                var awaits = new CompositeAwaitEvent(pressEvent, triggerEvent);
                yield return awaits;
                if ((pressEvent.Activated && pressEvent.KeyPressed == TakeKey) ||
                    (triggerEvent.Activated && triggerEvent.EventValue == UIEvent.TakePressed))
                {
                    break;
                }

                if ((pressEvent.Activated && pressEvent.KeyPressed == MulliganKey) ||
                    (triggerEvent.Activated && triggerEvent.EventValue == UIEvent.MulliganPressed))
                {
                    yield return MulliganCardsIntoDeck(props.Deck, cards);
                }

                Game.UI.ToggleMulliganPanel(false);
            }
            else
            {
                break;
            }
        }

        Game.UI.ToggleMulliganPanel(false);

        props.DrawResults.AddRange(cards);

        _dungeon.UnpauseActions();
    }

    private IEnumerator MulliganCardsIntoDeck<TCardType>(Deck<TCardType> deck, List<TCardType> cards) where TCardType : class, ICard
    {
        Game.Sounds.PlayClip(MulliganSound);
        Game.Player.GetPlayerStats().Mulligans--;
        yield return _animationController.AnimateShuffleCardsIntoDeck(cards, deck.DeckHolder);
        deck.PushToBottom(cards);
        Game.UI.UpdateEntityPanels();
    }

    private IEnumerator DoCardEvents<TCardType>(List<TCardType> cards, Func<TCardType, Routine> cardRoutine) where TCardType : ICard
    {
        // TODO TODO Left off here... need to find a way to put the event actions on the cards themselves
        var routineSet = new ParallelRoutineSet();
        foreach (var card in cards)
        {
            routineSet.AddRoutine(cardRoutine(card));
        }

        yield return routineSet.AsRoutine();
    }

    private class DrawCoroutineProps<TCardType> where TCardType : class, ICard
    {
        public DrawCoroutineProps(int numDraws, Deck<TCardType> deck, bool allowMulligan = true, Func<TCardType, bool> drawConditionFunc = null)
        {
            NumDraws = numDraws;
            Deck = deck;
            AllowMulligan = allowMulligan;
            DrawConditionFunc = drawConditionFunc;
        }

        /// <summary>
        /// Result of the draw process
        /// </summary>
        public List<TCardType> DrawResults { get; } = new List<TCardType>();

        public Func<TCardType, bool> DrawConditionFunc { get; }

        public int NumDraws { get; }

        public Deck<TCardType> Deck { get; }

        public bool AllowMulligan { get; }
    }
}