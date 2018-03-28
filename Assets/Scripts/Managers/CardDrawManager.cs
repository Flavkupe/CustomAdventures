

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using JetBrains.Annotations;

public class CardDrawManager : SingletonObject<CardDrawManager>
{
    public string TakeKey = " ";
    public string MulliganKey = "q";

    [UsedImplicitly]
    private void Awake()
    {
        Instance = this;
    }

    public Routine PerformDungeonEvents(RoomArea roomArea)
    {
        int draws = roomArea.NumDraws;
        if (roomArea.BossArea)
        {
            foreach (var bossCardData in Game.Dungeon.PossibleBossCards)
            {
                var bossCard = Game.Decks.CreateCardFromData<IDungeonCard, DungeonCardData>(bossCardData);
                Game.Decks.DungeonDeck.PushCard(bossCard);
            }

            draws = Game.Dungeon.PossibleBossCards.Length;
        }

        // TODO: empty deck?
        Func<IDungeonCard, Routine> cardExecuteRoutine = card =>
        {
            return GetDungeonCardRoutine(roomArea, card);
        };

        roomArea.gameObject.SetActive(false);
        Game.Dungeon.PostGroupEventCleanup();

        return DoCardDraw(new DrawCoroutineProps<IDungeonCard>(Game.Decks.DrawDungeonCards, draws, Game.Decks.DungeonDeck, cardExecuteRoutine, !roomArea.BossArea));
    }

    private Routine GetDungeonCardRoutine(RoomArea roomArea, IDungeonCard card)
    {
        switch (card.DungeonEventType)
        {
            case DungeonEventType.SpawnNear:
            case DungeonEventType.SpawnOnCorner:
            case DungeonEventType.SpawnOnWideOpen:
                return Routine.CreateAction(() => { Game.Dungeon.PerformSpawnEvent(roomArea, card); });
            case DungeonEventType.MultiEvent:
                return GenerateMultiEventRoutines(roomArea, card);
            default:
                Debug.LogError("No behavior set for DungeonEventType!");
                return Routine.Empty;
        }
    }

    private Routine GenerateMultiEventRoutines(RoomArea roomArea, IDungeonCard card)
    {
        var data = card.GetData<MultiEventCardData>();
        if (data == null)
        {
            Debug.LogError("Invalid cast for MultiEventCardData!");
            return Routine.Empty;
        }

        var drawChain = new ParallelRoutineSet();
        var effectChain = new ParallelRoutineSet();
        var events = new List<DungeonCardData>();
        if (data.MultiEventType == MultiEventType.DoEach)
        {
            events.AddRange(data.Events);
        }
        else
        {
            for (var i = 0; i < data.NumberOfEvents; i++)
            {
                events.Add(data.Events.GetRandom());
            }
        }

        var yDist = 1.2f;
        var zOffset = -0.2f;
        foreach (var subevent in events)
        {
            var newCard = Game.Decks.CreateAnonymousCardFromData<IDungeonCard>(subevent);
            if (newCard != null)
            {
                newCard.SetFaceUp();
                newCard.Object.transform.position = card.Object.transform.position.OffsetBy(0.0f, 0.0f, zOffset);
                drawChain.AddRoutine(Routine.Create(SlideCardUp, newCard, yDist));
                yDist += 1.2f;
                zOffset -= 0.2f;

                var cardRoutine = GetDungeonCardRoutine(roomArea, newCard);
                effectChain.AddRoutine(cardRoutine);
            }
        }

        var routine = drawChain.AsRoutine();
        routine.Then(() => Routine.WaitForSeconds(0.5f, true));
        routine.Then(() => effectChain);
        routine.Finally(card.DestroyCard);
        return routine;
    }

    private IEnumerator SlideCardUp(ICard card, float yDist)
    {
        Vector3 target = card.Object.transform.position + (Vector3.up * yDist);
        yield return card.Object.transform.MoveToSpotCoroutine(target, 10.0f);
    }

    public Routine PerformLootCardDrawing(int cardNum)
    {
        Func<ILootCard, Routine> func = card =>
        {
            switch (card.LootEventType)
            {
                case LootEventType.GainLoot:
                default:
                    return Routine.CreateAction(() =>
                    {
                        card.ExecuteLootGetEvent();
                        card.DestroyCard();
                    });
            }
        };      

        return DoCardDraw(Game.Decks.DrawLootCards, cardNum, Game.Decks.LootDeck, func);
    }

    public Routine PerformAbilityCardDrawing(int cardNum)
    {
        Func<IAbilityCard, Routine> func = card =>
        {
            return Routine.CreateAction(() =>
            {
                Game.Player.EquipAbility(card);
            });
        };

        return DoCardDraw(Game.Decks.DrawAbilityCards, cardNum, Game.Decks.AbilityDeck, func);
    }

    public Routine PerformCharacterCardDrawing(int cardNum)
    {
        Func<ICharacterCard, Routine> func = card =>
        {
            switch (card.CharacterCardType)
            {
                case CharacterCardType.AttributeGain:
                default:
                    return Routine.CreateAction(() =>
                    {
                        card.ApplyEffect();
                        card.DestroyCard();
                    });
            }
        };

        return DoCardDraw(Game.Decks.DrawCharacterCards, cardNum, Game.Decks.CharacterDeck, func);
    }

    private Routine DoCardDraw<TCardType>(DrawCoroutineProps<TCardType> props) where TCardType : class, ICard
    {
        Game.States.IsPaused = true;
        Routine drawRoutine = Routine.Create(InternalDrawCoroutine, props);
        Game.States.EnqueueIfNotState(GameState.CharacterMoving, () => drawRoutine);
        return drawRoutine;
    }

    private Routine DoCardDraw<TCardType>(Func<int, List<TCardType>> cardDrawFunc, int numDraws, Deck<TCardType> deck, Func<TCardType, Routine> cardRoutine) where TCardType : class, ICard
    {
        return DoCardDraw(new DrawCoroutineProps<TCardType>(cardDrawFunc, numDraws, deck, cardRoutine));
    }

    private class DrawCoroutineProps<TCardType> where TCardType : class, ICard
    {
        public DrawCoroutineProps(Func<int, List<TCardType>> cardDrawFunc, int numDraws, Deck<TCardType> deck, Func<TCardType, Routine> cardRoutine, bool allowMulligan = true)
        {
            CardDrawFunc = cardDrawFunc;
            NumDraws = numDraws;
            Deck = deck;
            CardRoutine = cardRoutine;
            AllowMulligan = allowMulligan;
        }

        public Func<int, List<TCardType>> CardDrawFunc { get; set; }

        public int NumDraws { get; private set; }

        public Deck<TCardType> Deck { get; private set; }

        public Func<TCardType, Routine> CardRoutine { get; private set; }

        public bool AllowMulligan { get; private set; }
    }

    private IEnumerator InternalDrawCoroutine<TCardType>(DrawCoroutineProps<TCardType> props) where TCardType : class, ICard
    {
        List<TCardType> cards;
        while (true)
        {
            cards = props.CardDrawFunc(props.NumDraws).ToList();
            yield return Game.Decks.AnimateCardDraws(cards.Cast<ICard>().ToList(), props.Deck.DeckHolder);
            if (props.AllowMulligan && Game.Player.Stats.Mulligans > 0)
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
                else if ((pressEvent.Activated && pressEvent.KeyPressed == MulliganKey) ||
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

        if (cards.Count > 0)
        {
            yield return DoCardEvents(cards, props.CardRoutine);
        }

        Game.States.IsPaused = false;
    }

    private IEnumerator MulliganCardsIntoDeck<TCardType>(Deck<TCardType> deck, List<TCardType> cards) where TCardType : class, ICard
    {
        Game.Player.Stats.Mulligans--;
        ParallelRoutineSet routines = new ParallelRoutineSet();
        foreach (var card in cards)
        {
            var routine = Routine.Create(Game.Decks.AnimateShuffleIntoDeck, card, deck.DeckHolder, 10.0f);
            routines.AddRoutine(routine);
        }

        yield return routines;

        deck.PushToBottom(cards);
        Game.UI.UpdateUI();
    }

    private IEnumerator DoCardEvents<TCardType>(List<TCardType> cards, Func<TCardType, Routine> cardRoutine) where TCardType : ICard
    {
        var routineChain = new RoutineChain();
        foreach (TCardType card in cards)
        {
            routineChain.AddRoutine(cardRoutine(card));
        }

        yield return StartCoroutine(routineChain.AsRoutine());
    }
}
