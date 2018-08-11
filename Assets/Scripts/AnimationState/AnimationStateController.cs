using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AnimationStateController : StateController<AnimationStateChangeContext>, IActionDeterminant<DungeonActionType>
{
    private BlockingAnimationState blockingAnimationsState;
    private NoAnimationState idleState;

    public AnimationStateController() : base("Animation")
    {
        blockingAnimationsState = new BlockingAnimationState(this);
        idleState = new NoAnimationState(this);
        
        blockingAnimationsState.AddTransitions(new[]
        {
            new AnimationStateTransition(AnimationDecision.Decisions.DidAllAnimationsEndDecision, idleState)
        });

        idleState.AddTransitions(new[]
        {
            new AnimationStateTransition(AnimationDecision.Decisions.DidAnimationStartDecision, blockingAnimationsState)
        });

        FirstState = idleState;
        CurrentState = FirstState;
    }

    /// <summary>
    /// Enqueues an animation and sets the state to animate. State enqueued to revert
    /// once animation ends.
    /// </summary>
    public void AddAnimationRoutine(Routine animation, GameContext context)
    {
        animation.Finally(() => SendEvent(AnimationEventType.AnimationEnd, context));
        EnqueueRoutine(animation);
        SendEvent(AnimationEventType.AnimationStart, context);
    }

    public void SendEvent(AnimationEventType eventType, GameContext context)
    {
        var eventContext = new AnimationStateChangeContext(eventType, context, this);
        EventOccurred(eventContext);
    }

    public bool CanPerformAction(DungeonActionType actionType)
    {
        return CanPerformActionInState(actionType);
    }
}