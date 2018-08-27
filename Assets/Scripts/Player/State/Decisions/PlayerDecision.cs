
using Assets.Scripts.Player.State.Context;

public abstract class PlayerDecision : Decision<PlayerStateChangeContext>
{
    public class Decisions
    {
        public static DidPlayerTurnStart DidPlayerTurnStart => new DidPlayerTurnStart();
        public static DidPlayerTurnEnd DidPlayerTurnEnd => new DidPlayerTurnEnd();
        public static DidCombatEnd DidCombatEnd => new DidCombatEnd();
        public static DidEventOccur<PlayerStateChangeContext, PlayerEventType> DidEventOccur(PlayerEventType eventType)
        {
            return new DidEventOccur<PlayerStateChangeContext, PlayerEventType>(eventType, a => a.EventType);
        }
    }

    public class DidPlayerTurnStart : PlayerDecision
    {
        public override bool Evaluate(PlayerStateChangeContext context)
        {
            var gameContext = context.GameContext;
            if (context.EventType == PlayerEventType.AITurnsComplete ||
                context.EventType == PlayerEventType.InitializeCombat)
            {
                if (gameContext.Dungeon.IsCombat)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class DidPlayerTurnEnd : PlayerDecision
    {
        public override bool Evaluate(PlayerStateChangeContext context)
        {
            var gameContext = context.GameContext;
            if (context.EventType == PlayerEventType.AfterMove)
            {
                return !gameContext.Player.PlayerHasMoves;
            }

            return false;
        }
    }

    public class DidCombatEnd : PlayerDecision
    {
        public override bool Evaluate(PlayerStateChangeContext context)
        {
            return !context.GameContext.Dungeon.IsCombat;
        }
    }
}
