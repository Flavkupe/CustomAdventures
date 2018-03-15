

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

public class CardDrawManager : SingletonObject<CardDrawManager>
{
    public string TakeKey = " ";
    public string MulliganKey = "q";

    void Awake()
    {
        Instance = this;
    }

    public Routine PerformDungeonEvents(RoomArea roomArea)
    {
        // TODO: empty deck?        
        Func<IDungeonCard, Routine> func = (IDungeonCard card) =>
        {
            switch (card.DungeonEventType)
            {
                case DungeonEventType.SpawnNear:
                case DungeonEventType.SpawnOnCorner:
                case DungeonEventType.SpawnOnWideOpen:
                    return Routine.Create(() =>
                    {
                        Game.Dungeon.PerformSpawnEvent(roomArea, card);
                    });
                default:
                    Debug.LogError("No behavior set for DungeonEventType!");
                    return null;
            }
        };

        roomArea.gameObject.SetActive(false);
        Game.Dungeon.PostGroupEventCleanup();

        return DoCardDraw(Game.Decks.DrawDungeonCards, 2, Game.Decks.DungeonDeck, func);
    }

    public Routine PerformLootCardDrawing(int cardNum)
    {
        Func<ILootCard, Routine> func = (ILootCard card) =>
        {
            switch (card.LootEventType)
            {
                case LootEventType.GainLoot:
                default:
                    return Routine.Create(() =>
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
        Func<IAbilityCard, Routine> func = (IAbilityCard card) =>
        {
            return Routine.Create(() =>
            {
                Player.Instance.EquipAbility(card);                
            });
        };

        return DoCardDraw(Game.Decks.DrawAbilityCards, cardNum, Game.Decks.AbilityDeck, func);
    }

    public Routine PerformCharacterCardDrawing(int cardNum)
    {
        Func<ICharacterCard, Routine> func = (ICharacterCard card) =>
        {
            switch (card.CharacterCardType)
            {
                case CharacterCardType.AttributeGain:
                default:
                    return Routine.Create(() =>
                    {
                        card.ApplyEffect();
                        card.DestroyCard();
                    });
            }
        };

        return DoCardDraw(Game.Decks.DrawCharacterCards, cardNum, Game.Decks.CharacterDeck, func);
    }

    private Routine DoCardDraw<TCardType>(Func<int, List<TCardType>> cardDrawFunc, int numDraws, Deck<TCardType> deck, Func<TCardType, Routine> cardRoutine) where TCardType : class, ICard
    {
        Game.States.IsPaused = true;

        Routine drawRoutine = Routine.Create(InternalDrawCoroutine, cardDrawFunc, numDraws, deck, cardRoutine);        
        Game.States.EnqueueIfNotState(GameState.CharacterMoving, () => drawRoutine);        
        return drawRoutine;
    }

    private IEnumerator InternalDrawCoroutine<TCardType>(Func<int, List<TCardType>> cardDrawFunc, int numDraws, Deck<TCardType> deck, Func<TCardType, Routine> cardRoutine) where TCardType : class, ICard
    {
        List<TCardType> cards = null;
        while (true)
        {
            cards = cardDrawFunc(numDraws).ToList();
            yield return StartCoroutine(Game.Decks.AnimateCardDraws(cards.Cast<ICard>().ToList(), deck.DeckHolder, 10.0f));
            if (Game.Player.Stats.Mulligans > 0)
            {
                Game.UI.ToggleMulliganPanel(true);
                var pressEvent = new AwaitKeyPress(MulliganKey, TakeKey);
                var triggerEvent = new AwaitTriggerEvent<UIEvent>(Game.UI.GetCurrentUIEvent, UIEvent.MulliganPressed, UIEvent.TakePressed);
                var awaits = new CompositeAwaitEvent(pressEvent, triggerEvent);
                yield return StartCoroutine(awaits);
                if ((pressEvent.Activated && pressEvent.KeyPressed == TakeKey) || 
                    (triggerEvent.Activated && triggerEvent.EventValue == UIEvent.TakePressed))
                {
                    break;
                }
                else if ((pressEvent.Activated && pressEvent.KeyPressed == MulliganKey) ||
                         (triggerEvent.Activated && triggerEvent.EventValue == UIEvent.MulliganPressed))
                {
                    yield return StartCoroutine(MulliganCardsIntoDeck(deck, cards));                    
                }

                Game.UI.ToggleMulliganPanel(false);
            }
            else
            {
                break;
            }
        }

        Game.UI.ToggleMulliganPanel(false);
        Debug.Assert(cards != null, "Card are null!");

        if (cards != null && cards.Count > 0)
        {
            yield return StartCoroutine(DoCardEvents(cards, cardRoutine));
        }

        Game.States.IsPaused = false;
    }

    private IEnumerator MulliganCardsIntoDeck<TCardType>(Deck<TCardType> deck, List<TCardType> cards) where TCardType : class, ICard
    {
        Game.Player.Stats.Mulligans--;
        ParallelRoutineSet routines = new ParallelRoutineSet(StartCoroutine);
        foreach (var card in cards)
        {
            var routine = Routine.Create(Game.Decks.AnimateShuffleIntoDeck, card, deck.DeckHolder, 10.0f);
            routines.AddRoutine(routine);
        }

        yield return StartCoroutine(routines);

        deck.PushToBottom(cards);
        Game.UI.UpdateUI();        
    }

    private IEnumerator DoCardEvents<TCardType>(List<TCardType> cards, Func<TCardType, Routine> cardRoutine) where TCardType : ICard
    {
        var routineChain = new RoutineChain();
        foreach (TCardType card in cards)
        {
            routineChain.Enqueue(cardRoutine(card));
        }

        yield return StartCoroutine(routineChain.AsRoutine());
    }
}
