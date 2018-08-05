using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AnimationStateController : StateController<AnimationStateChangeContext>, IActionDeterminant<DungeonActionType>
{
    public AnimationStateController() : base("Animation")
    {
        var cardDrawState = new CardDrawingState(this);
        var idleState = new NoAnimationState(this);

        cardDrawState.AddTransitions(new[]
        {
            new AnimationStateTransition(AnimationDecision.Decisions.AreCardsDoneMovingDecision, idleState)
        });

        idleState.AddTransitions(new[]
        {
            new AnimationStateTransition(AnimationDecision.Decisions.DidCardsStartMovingDecision, cardDrawState)
        });   

        FirstState = idleState;
        CurrentState = FirstState;
    }

    public void SendEvent(AnimationEventType eventType, GameContext context)
    {
        var eventContext = new AnimationStateChangeContext(eventType, context);
        EventOccurred(eventContext);
    }

    public bool CanPerformAction(DungeonActionType actionType)
    {
        return CanPerformActionInState(actionType);
    }
}