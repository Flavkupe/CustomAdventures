﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class AnimationDecision : Decision<AnimationStateChangeContext>
{
    public class Decisions
    {
        public static AreCardsDoneMovingDecision AreCardsDoneMovingDecision => new AreCardsDoneMovingDecision();
        public static DidCardsStartMovingDecision DidCardsStartMovingDecision => new DidCardsStartMovingDecision();
    }
}

public class AreCardsDoneMovingDecision : AnimationDecision
{
    public override bool Evaluate(AnimationStateChangeContext context)
    {
        return context.EventType == AnimationEventType.AnimationEnd;
    }
}

public class DidCardsStartMovingDecision : AnimationDecision
{
    public override bool Evaluate(AnimationStateChangeContext context)
    {
        return context.EventType == AnimationEventType.AnimationStart;
    }
}
