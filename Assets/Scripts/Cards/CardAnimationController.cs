using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardEventController))]
public class CardAnimationController : MonoBehaviourEx
{
    public AudioClip[] DrawSounds;

    public float DeckSmallSize = 0.35f;
    public float DeckBigSize = 1.00f;
    public GameObject CardDrawPos;

    public float CardMoveSpeed = 15.0f;

    public event EventHandler CardDrawStart;
    public event EventHandler CardDrawEnd;

    public IEnumerator AnimateCardDraws(IEnumerable<ICard> cards, GameObject deckHolder, float deckMoveSpeed = 10.0f)
    {
        CardDrawStart?.Invoke(null, null);
        float targetX = 0.0f;
        Vector3 initPos = deckHolder.transform.position;
        yield return MoveDeckToPosition(deckHolder, CardDrawPos.transform.position, DeckBigSize - DeckSmallSize, deckMoveSpeed);

        ParallelRoutineSet routineSet = new ParallelRoutineSet();

        foreach (var card in cards)
        {
            targetX += 3.0f;
            routineSet.AddRoutine(Routine.Create(AnimateIndividualCardDraw, card, targetX));
        }

        Game.Sounds.PlayFromClips(DrawSounds);

        yield return routineSet;

        yield return new WaitForSecondsSpeedable(1.0f);

        yield return MoveDeckToPosition(deckHolder, initPos, DeckSmallSize - DeckBigSize);

        yield return new WaitForSecondsSpeedable(0.5f);

        CardDrawEnd?.Invoke(null, null);
    }

    private IEnumerator MoveDeckToPosition(GameObject deckHolder, Vector3 target, float sizeChange, float deckMoveSpeed = 10.0f)
    {
        yield return deckHolder.transform.MoveToSpotAndScaleCoroutine(target, deckMoveSpeed, sizeChange);
    }

    private IEnumerator AnimateIndividualCardDraw(ICard card, float targetX)
    {
        MonoBehaviourEx obj = card.Object;
        Vector3 target = obj.transform.position.IncrementBy(-targetX, 0.0f, 0.0f);
        yield return obj.transform.MoveToSpotCoroutine(target, CardMoveSpeed);
        yield return obj.transform.RotateCoroutine(Vector3.up, 180.0f, 200.0f);
        obj.transform.eulerAngles = new Vector3(0.0f, 0.0f);
        obj.transform.SetParent(null);
    }

    public IEnumerator AnimateShuffleIntoDeck(ICard card, GameObject deckHolder)
    {
        MonoBehaviourEx obj = card.Object;
        yield return obj.transform.MoveToSpotAndScaleCoroutine(deckHolder.transform.position, CardMoveSpeed, DeckSmallSize - DeckBigSize);
        yield return obj.transform.RotateCoroutine(Vector3.up, 0.0f, 200.0f);
        obj.transform.eulerAngles = new Vector3(0.0f, 0.0f);
        obj.transform.SetParent(deckHolder.transform);
    }

    public IEnumerator AnimateShuffleCardsIntoDeck(IEnumerable<ICard> cards, GameObject deckHolder)
    {
        ParallelRoutineSet routines = new ParallelRoutineSet();
        foreach (var card in cards)
        {
            var routine = Routine.Create(AnimateShuffleIntoDeck, card, deckHolder);
            routines.AddRoutine(routine);
        }

        yield return routines;
    }

    public IEnumerator AnimateQuickSlideupDraw(ICard card)
    {
        yield return SlideCardUp(card, 1.5f, 15.0f);
        yield return Routine.WaitForSeconds(0.3f, true);
        yield return card.Object.transform.RotateCoroutine(Vector3.up, 180, 500.0f);
        yield return Routine.WaitForSeconds(0.3f, true);
    }

    private IEnumerator SlideCardUp(ICard card, float yDist, float speed)
    {
        var target = card.Object.transform.position + (Vector3.up * yDist);
        yield return card.Object.transform.MoveToSpotCoroutine(target, speed);
    }
}
