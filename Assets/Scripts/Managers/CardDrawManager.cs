

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

public class CardDrawManager : SingletonObject<CardDrawManager>
{
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

        return DoCardDraw(Game.Decks.DrawDungeonCards, 2, Game.Decks.DungeonDeckHolder, func);
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

        return DoCardDraw(Game.Decks.DrawLootCards, cardNum, Game.Decks.LootDeckHolder, func);
    }

    public Routine PerformAbilityCardDrawing(int cardNum)
    {
        Func<IAbilityCard, Routine> func = (IAbilityCard card) =>
        {
            return Routine.Create(() =>
            {
                Player.Instance.EquipAbility(card);
                card.DestroyCard();
            });
        };

        return DoCardDraw(Game.Decks.DrawAbilityCards, cardNum, Game.Decks.AbilityDeckHolder, func);
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

        return DoCardDraw(Game.Decks.DrawCharacterCards, cardNum, Game.Decks.CharDeckHolder, func);
    }

    private Routine DoCardDraw<TCardType>(Func<int, List<TCardType>> cardDrawFunc, int numDraws, GameObject deckHolder, Func<TCardType, Routine> cardRoutine) where TCardType : ICard
    {
        Game.States.IsPaused = true;

        Routine drawRoutine = Routine.Create(InternalDrawCoroutine, cardDrawFunc, numDraws, deckHolder, cardRoutine);        
        Game.States.EnqueueIfNotState(GameState.CharacterMoving, () => drawRoutine);        
        return drawRoutine;
    }

    private IEnumerator InternalDrawCoroutine<TCardType>(Func<int, List<TCardType>> cardDrawFunc, int numDraws, GameObject deckHolder, Func<TCardType, Routine> cardRoutine) where TCardType : ICard
    {
        List<TCardType> cards = null;
        while (true)
        {
            cards = cardDrawFunc(numDraws).ToList();
            yield return StartCoroutine(Game.Decks.AnimateCardDraws(cards.Cast<ICard>().ToList(), deckHolder, 10.0f));
            if (Game.Player.Stats.Mulligans > 0)
            {
                AwaitKeyPress pressEvent = new AwaitKeyPress();
                yield return StartCoroutine(pressEvent);
                if (pressEvent.KeyPressed != "q")
                {
                    break;
                }
                else
                {
                    Game.Player.Stats.Mulligans--;
                    Game.UI.UpdateUI();

                    // TODO: shuffle back in; don't destroy!
                    foreach (var card in cards)
                    {
                        card.DestroyCard();
                    }
                }
            }
            else
            {
                break;
            }
        }

        Debug.Assert(cards != null, "Card are null!");

        if (cards != null && cards.Count > 0)
        {
            yield return StartCoroutine(DoCardEvents(cards, cardRoutine));
        }

        Game.States.IsPaused = false;
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

    
    private class AwaitKeyPress : CustomYieldInstruction
    {
        public string KeyPressed { get; set; }
        public override bool keepWaiting
        {
            get
            {
                
                if (Input.anyKeyDown && !string.IsNullOrEmpty(Input.inputString))
                {
                    this.KeyPressed = Input.inputString;
                    return false;
                }

                return true;
            }
        }
    }
}
