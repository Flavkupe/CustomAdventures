using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class DungeonDecision : Decision<DungeonStateChangeContext>
{



    public class Decisions
    {
        public static AreCardsDoneMovingDecision AreCardsDoneMovingDecision => new AreCardsDoneMovingDecision();
        public static DidCardsStartMovingDecision DidCardsStartMovingDecision => new DidCardsStartMovingDecision();
    }
}

public class AreCardsDoneMovingDecision : DungeonDecision
{
    public override bool Evaluate(DungeonStateChangeContext context)
    {
        return context.EventType == DungeonEventType.CardDrawComplete;
    }
}

public class DidCardsStartMovingDecision : DungeonDecision
{
    public override bool Evaluate(DungeonStateChangeContext context)
    {
        return context.EventType == DungeonEventType.CardDrawStarted;
    }
}
