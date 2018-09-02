using Assets.Scripts.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class DungeonDecision : Decision<DungeonEventType>
{
    public class Decisions : Decisions<DungeonEventType>
    {
        public static DidEnemyTurnStart DidEnemyTurnStart => new DidEnemyTurnStart();
        public static DidEnemyTurnEnd DidEnemyTurnEnd => new DidEnemyTurnEnd();
    }

    public class DidEnemyTurnStart : DungeonDecision
    {
        public override bool Evaluate(StateContext<DungeonEventType> context)
        {
            var gameContext = context.GameContext;
            if (context.Event == DungeonEventType.AfterPlayerAction)
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
        public override bool Evaluate(StateContext<DungeonEventType> context)
        {
            return context.Event == DungeonEventType.AllEnemiesTurnEnd;
        }
    }
}