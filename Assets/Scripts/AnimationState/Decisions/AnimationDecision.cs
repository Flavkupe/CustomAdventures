using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class AnimationDecision : Decision<AnimationStateChangeContext>
{
    public class Decisions
    {
        public static DidAllAnimationsEndDecision DidAllAnimationsEndDecision => new DidAllAnimationsEndDecision();
        public static DidAnimationStartDecision DidAnimationStartDecision => new DidAnimationStartDecision();
        public static DidEventOccur<AnimationStateChangeContext, AnimationEventType> DidEventOccur(AnimationEventType eventType)
        {
            return new DidEventOccur<AnimationStateChangeContext, AnimationEventType>(eventType, a => a.EventType);
        }
    }
}

public class DidAllAnimationsEndDecision : AnimationDecision
{
    public override bool Evaluate(AnimationStateChangeContext context)
    {
        // Animation is actually done when AnimationEnd message passes and no more animations are in queue
        return context.EventType == AnimationEventType.AnimationEnd && context.Controller.QueueIdle;
    }
}

public class DidAnimationStartDecision : AnimationDecision
{
    public override bool Evaluate(AnimationStateChangeContext context)
    {
        return context.EventType == AnimationEventType.AnimationStart;
    }
}


