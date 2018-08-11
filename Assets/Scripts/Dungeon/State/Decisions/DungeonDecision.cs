using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class DungeonDecision : Decision<DungeonStateChangeContext>
{
    public class Decisions
    {
        public static DidEnemyTurnStart DidEnemyTurnStart => new DidEnemyTurnStart();
        public static DidEnemyTurnEnd DidEnemyTurnEnd => new DidEnemyTurnEnd();

        public static DidEventOccur<DungeonStateChangeContext, DungeonEventType> DidEventOccur(DungeonEventType eventType)
        {
            return new DidEventOccur<DungeonStateChangeContext, DungeonEventType>(eventType, a => a.EventType);
        }
    }

    public class DidEnemyTurnStart : DungeonDecision
    {
        public override bool Evaluate(DungeonStateChangeContext context)
        {
            var gameContext = context.GameContext;
            if (context.EventType == DungeonEventType.AfterPlayerAction)
            {
                if (gameContext.Dungeon.IsCombat && !gameContext.Player.PlayerHasMoves)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class DidEnemyTurnEnd : DungeonDecision
    {
        public override bool Evaluate(DungeonStateChangeContext context)
        {
            return context.EventType == DungeonEventType.AllEnemiesTurnEnd;
        }
    }
}