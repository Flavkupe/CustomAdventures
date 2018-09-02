
using Assets.Scripts.State;

public abstract class PlayerDecision : Decision<PlayerEventType>
{
    public class Decisions : Decisions<PlayerEventType>
    {
        public static DidPlayerTurnStart DidPlayerTurnStart => new DidPlayerTurnStart();
        public static IsPlayerOutOfMoves IsPlayerOutOfMoves => new IsPlayerOutOfMoves();
        public static DidCombatEnd DidCombatEnd => new DidCombatEnd();
    }

    public class DidPlayerTurnStart : PlayerDecision
    {
        public override bool Evaluate(StateContext<PlayerEventType> context)
        {
            var gameContext = context.GameContext;
            return (context.Event == PlayerEventType.AITurnsComplete ||
                context.Event == PlayerEventType.InitializeCombat) &&
                gameContext.Dungeon.IsCombat;
        }
    }

    public class IsPlayerOutOfMoves : PlayerDecision
    {
        public override bool Evaluate(StateContext<PlayerEventType> context)
        {
            return !context.GameContext.Player.PlayerHasMoves;
        }
    }

    public class DidCombatEnd : PlayerDecision
    {
        public override bool Evaluate(StateContext<PlayerEventType> context)
        {
            return !context.GameContext.Dungeon.IsCombat;
        }
    }
}
