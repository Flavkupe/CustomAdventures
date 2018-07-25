using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class DungeonDecision : Decision<DungeonStateChangeContext>
{
    public class Decisions
    {
        public static DidEnemyTurnStartDecision DidEnemyTurnStartDecision => new DidEnemyTurnStartDecision();
        public static DidEnemyTurnEndDecision DidEnemyTurnEndDecision => new DidEnemyTurnEndDecision();
    }
}

public class DidEnemyTurnStartDecision : DungeonDecision
{
    public override bool Evaluate(DungeonStateChangeContext context)
    {
        return context.EventType == DungeonEventType.EnemyTurnStart;
    }
}

public class DidEnemyTurnEndDecision : DungeonDecision
{
    public override bool Evaluate(DungeonStateChangeContext context)
    {
        return context.EventType == DungeonEventType.EnemyTurnEnd;
    }
}