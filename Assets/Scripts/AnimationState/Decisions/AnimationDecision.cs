using Assets.Scripts.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class AnimationDecision : Decision<AnimationEventType>
{
    public class Decisions : Decisions<AnimationEventType>
    {
        public static DidAllAnimationsEndDecision DidAllAnimationsEndDecision => new DidAllAnimationsEndDecision();
        public static DidAnimationStartDecision DidAnimationStartDecision => new DidAnimationStartDecision();
    }
}

public class DidAllAnimationsEndDecision : AnimationDecision
{
    public override bool EvaluateEvent(StateContext<AnimationEventType> context)
    {
        // Animation is actually done when AnimationEnd message passes and no more animations are in queue
        return context.Event == AnimationEventType.AnimationEnd && context.CurrentState.Controller.QueueIdle;
    }
}

public class DidAnimationStartDecision : AnimationDecision
{
    public override bool EvaluateEvent(StateContext<AnimationEventType> context)
    {
        return context.Event == AnimationEventType.AnimationStart;
    }
}


