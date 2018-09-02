using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AnimationStateController : StateController<AnimationState, AnimationEventType>, IActionDeterminant<DungeonActionType>
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
        animation.Finally(() => HandleNewEvent(AnimationEventType.AnimationEnd, context));
        EnqueueRoutine(animation);
        HandleNewEvent(AnimationEventType.AnimationStart, context);
    }

    public bool CanPerformAction(DungeonActionType actionType)
    {
        return CanPerformActionInState(actionType);
    }
}