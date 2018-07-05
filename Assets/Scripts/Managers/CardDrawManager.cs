

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

    public AudioClip MulliganSound;

    [UsedImplicitly]
    private void Awake()
    {
        Instance = this;
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

        var routine = Routine.Create(QuickDrawCoroutine, new DrawCoroutineProps<IAbilityCard>(cardNum, Game.Decks.AbilityDeck, func));
        // Game.States.EnqueueIfNotState(GameState.CharacterMoving, () => routine);
        StartCoroutine(routine);
        return routine;
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

        return DoCardDraw(cardNum, Game.Decks.CharacterDeck, func);
    }

    private Routine DoCardDraw<TCardType>(DrawCoroutineProps<TCardType> props) where TCardType : class, ICard
    {
        Game.States.IsPaused = true;
        Routine drawRoutine = Routine.Create(InternalDrawCoroutine, props);

        // This is where the routine is executed!
        Game.States.EnqueueIfNotState(GameState.CharacterMoving, () => drawRoutine);
        return drawRoutine;
    }

    private Routine DoCardDraw<TCardType>(int numDraws, Deck<TCardType> deck, Func<TCardType, Routine> cardRoutine) where TCardType : class, ICard
    {
        return DoCardDraw(new DrawCoroutineProps<TCardType>(numDraws, deck, cardRoutine));
    }

    private class DrawCoroutineProps<TCardType> where TCardType : class, ICard
    {
        public DrawCoroutineProps(int numDraws, Deck<TCardType> deck, 
            Func<TCardType, Routine> cardRoutine, bool allowMulligan = true, Func<TCardType, bool> drawConditionFunc = null)
        {
            NumDraws = numDraws;
            Deck = deck;
            CardRoutine = cardRoutine;
            AllowMulligan = allowMulligan;
            DrawConditionFunc = drawConditionFunc;
        }

        public Func<TCardType, bool> DrawConditionFunc { get; }

        public int NumDraws { get; }

        public Deck<TCardType> Deck { get; }

        public Func<TCardType, Routine> CardRoutine { get; }

        public bool AllowMulligan { get; }
    }

    private IEnumerator QuickDrawCoroutine<TCardType>(DrawCoroutineProps<TCardType> props) where TCardType : class, ICard
    {
        List<TCardType> cards = Game.Decks.DrawCards(props.NumDraws, props.Deck).ToList();
        var fullSize = Game.Decks.DeckBigSize;

        foreach (var card in cards)
        {
            yield return SlideCardUp(card, 1.5f, 20.0f);
            yield return Routine.WaitForSeconds(0.5f, true);
            yield return card.Object.transform.RotateCoroutine(Vector3.up, 180, 500.0f);
            yield return Routine.WaitForSeconds(0.5f, true);
            yield return props.CardRoutine(card);

            // Make card big for UI display
            card.Object.transform.localScale = new Vector3(fullSize, fullSize, fullSize);
        }
    }

    private IEnumerator SlideCardUp(ICard card, float yDist, float speed)
    {
        var target = card.Object.transform.position + (Vector3.up * yDist);
        yield return card.Object.transform.MoveToSpotCoroutine(target, speed);
    }

    private IEnumerator InternalDrawCoroutine<TCardType>(DrawCoroutineProps<TCardType> props) where TCardType : class, ICard
    {
        List<TCardType> cards;
        while (true)
        {
            cards = Game.Decks.DrawCards(props.NumDraws, props.Deck, props.DrawConditionFunc).ToList();
            yield return Game.Decks.AnimateCardDraws(cards.Cast<ICard>().ToList(), props.Deck.DeckHolder);
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

        if (cards.Count > 0)
        {
            yield return DoCardEvents(cards, props.CardRoutine);
        }

        Game.States.IsPaused = false;
    }

    private IEnumerator MulliganCardsIntoDeck<TCardType>(Deck<TCardType> deck, List<TCardType> cards) where TCardType : class, ICard
    {
        Game.Sounds.PlayClip(MulliganSound);
        Game.Player.GetPlayerStats().Mulligans--;
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
        var routineSet = new ParallelRoutineSet();
        foreach (TCardType card in cards)
        {
            routineSet.AddRoutine(cardRoutine(card));
        }

        yield return routineSet.AsRoutine();
    }
}
