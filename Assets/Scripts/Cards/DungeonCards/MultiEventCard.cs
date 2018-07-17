using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiEventCard : DungeonCard<MultiEventCardData>
{
    public override IEnumerator ExecuteDungeonEvent(DungeonCardExecutionContext context)
    {
        var drawChain = new ParallelRoutineSet();
        var effectChain = new ParallelRoutineSet();
        var events = new List<DungeonCardData>();
        if (Data.MultiEventType == MultiEventType.DoEach)
        {
            events.AddRange(Data.Events);
        }
        else
        {
            for (var i = 0; i < Data.NumberOfEvents; i++)
            {
                events.Add(Data.Events.GetRandom());
            }
        }

        var yDist = 1.2f;
        var zOffset = -0.2f;
        var cards = new List<IDungeonCard>();
        foreach (var subevent in events)
        {
            // Create the cards
            var newCard = subevent.CreateAnonymousCardFromData<IDungeonCard>();
            if (newCard != null)
            {
                cards.Add(newCard);
            }
        }
        
        foreach (var card in cards)
        {
            // Animate cards
            card.SetFaceUp();
            card.Object.transform.position = transform.position.OffsetBy(0.0f, 0.0f, zOffset);
            drawChain.AddRoutine(Routine.Create(SlideCardUp, card, yDist, 10.0f));
            yDist += 1.2f;
            zOffset -= 0.2f;
            
            // Queue up execution to happen after animation
            var cardRoutine = Routine.Create(card.ExecuteDungeonEvent, context);
            cardRoutine.Finally(card.DestroyCard);
            effectChain.AddRoutine(cardRoutine);
        }       

        var routine = drawChain.AsRoutine();
        routine.Then(() => Routine.WaitForSeconds(0.5f, true));
        routine.Then(() => ToggleHideCard(true));
        routine.Then(effectChain.AsRoutine());
        yield return routine;
    }

    private IEnumerator SlideCardUp(ICard card, float yDist, float speed)
    {
        var target = card.Object.transform.position + (Vector3.up * yDist);
        yield return card.Object.transform.MoveToSpotCoroutine(target, speed);
    }
}
