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
        public static DidSelectionEndTurn DidSelectionEndTurn => new DidSelectionEndTurn();
        public static DidSelectionNotEndTurn DidSelectionNotEndTurn => new DidSelectionNotEndTurn();
    }

    public class DidEnemyTurnStart : DungeonDecision
    {
        public override bool EvaluateEvent(StateContext<DungeonEventType> context)
        {
            return false;
        }

        public override bool EvaluateContext(GameContext gameContext)
        {
            if (gameContext.Dungeon.IsCombat && !gameContext.Player.PlayerHasMoves)
            {
                return true;
            }

            return false;
        }
    }

    public class DidEnemyTurnEnd : DungeonDecision
    {
        public override bool EvaluateEvent(StateContext<DungeonEventType> context)
        {
            return context.Event == DungeonEventType.AllEnemiesTurnEnd;
        }
    }

    public class DidSelectionEndTurn : DungeonDecision
    {
        public override bool EvaluateEvent(StateContext<DungeonEventType> context)
        {
            if (context.Event == DungeonEventType.SelectionCancelled)
            {
                return false;
            }
            
            if (context.GameContext.Player.PlayerHasMoves)
            {
                return false;
            }

            return true;
        }
    }

    public class DidSelectionNotEndTurn : DidSelectionEndTurn
    {
        public override bool EvaluateEvent(StateContext<DungeonEventType> context)
        {
            return !base.EvaluateEvent(context);
        }
    }
}