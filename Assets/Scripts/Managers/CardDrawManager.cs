

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardDrawManager : SingletonObject<CardDrawManager>
{
    void Awake()
    {
        Instance = this;
    }

    public Routine PerformDungeonEvents(RoomArea roomArea)
    {
        // TODO: empty deck?
        List<IDungeonCard> cards = new List<IDungeonCard>();
        cards = DeckManager.Instance.DrawDungeonCards(2);
        var routineChain = new RoutineChain();
        foreach (IDungeonCard card in cards)
        {
            switch (card.DungeonEventType)
            {
                case DungeonEventType.SpawnNear:
                case DungeonEventType.SpawnOnCorner:
                case DungeonEventType.SpawnOnWideOpen:
                    routineChain.Enqueue(() =>
                    {
                        Game.Dungeon.PerformSpawnEvent(roomArea, card);
                    });
                    break;
                default:
                    Debug.LogError("No behavior set for DungeonEventType!");
                    break;
            }

        }

        roomArea.gameObject.SetActive(false);
        Game.Dungeon.PostGroupEventCleanup();

        return DoCardDraw(cards.Cast<ICard>().ToList(), Game.Decks.DungeonDeckHolder, routineChain.AsRoutine());
    }

    public Routine PerformLootCardDrawing(int cardNum)
    {
        var lootCards = DeckManager.Instance.DrawLootCards(cardNum);
        var routineChain = new RoutineChain();
        foreach (ILootCard card in lootCards)
        {
            switch (card.LootEventType)
            {
                case LootEventType.GainLoot:
                default:
                    routineChain.Enqueue(Routine.Create(() =>
                    {
                        card.ExecuteLootGetEvent();
                        card.DestroyCard();
                    }));

                    break;
            }
        }

        return DoCardDraw(lootCards.Cast<ICard>().ToList(), Game.Decks.LootDeckHolder, routineChain.AsRoutine());
    }

    public Routine PerformAbilityCardDrawing(int cardNum)
    {
        var abilityCards = DeckManager.Instance.DrawAbilityCards(cardNum);
        var routineChain = new RoutineChain();
        foreach (IAbilityCard card in abilityCards)
        {
            routineChain.Enqueue(Routine.Create(() =>
            {
                Player.Instance.EquipAbility(card);
                card.DestroyCard();
            }));
        }

        return DoCardDraw(abilityCards.Cast<ICard>().ToList(), Game.Decks.AbilityDeckHolder, routineChain.AsRoutine());
    }

    public Routine PerformCharacterCardDrawing(int cardNum)
    {
        var charCards = DeckManager.Instance.DrawCharacterCards(cardNum);
        var routineChain = new RoutineChain();
        foreach (ICharacterCard card in charCards)
        {
            switch (card.CharacterCardType)
            {
                case CharacterCardType.AttributeGain:
                default:
                    routineChain.Enqueue(Routine.Create(() =>
                    {
                        card.ApplyEffect();
                        card.DestroyCard();
                    }));

                    break;
            }
        }

        return DoCardDraw(charCards.Cast<ICard>().ToList(), Game.Decks.CharDeckHolder, routineChain.AsRoutine());
    }

    private Routine DoCardDraw(List<ICard> cards, GameObject deckHolder, Routine afterDone = null)
    {
        StateManager.Instance.IsPaused = true;

        Routine drawRoutine = Routine.Create(Game.Decks.AnimateCardDraws, cards, deckHolder, 10.0f);
        drawRoutine.Then(() =>
        {
            StateManager.Instance.IsPaused = false;
        });

        if (afterDone != null)
        {
            drawRoutine.Then(afterDone);
        }

        StateManager.Instance.EnqueueIfNotState(GameState.CharacterMoving, () => drawRoutine);
        return drawRoutine;
    }
}
