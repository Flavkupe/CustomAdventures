using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerStateController : StateController<PlayerState, PlayerEventType>, IActionDeterminant<DungeonActionType>
{
    public PlayerStateController() : base("Player")
    {
        var exploreState = new PlayerExploreState(this);
        var awaitingTurnState = new PlayerAwaitingTurnState(this);
        var combatTurnState = new PlayerCombatTurnState(this);

        exploreState.AddTransitions(new[]
        {
            new PlayerStateTransition(PlayerDecision.Decisions.DidPlayerTurnStart, combatTurnState),
        });

        awaitingTurnState.AddTransitions(new[]
        {
            new PlayerStateTransition(PlayerDecision.Decisions.DidPlayerTurnStart, combatTurnState)
        });

        combatTurnState.AddTransitions(new[]
        {
            new PlayerStateTransition(PlayerDecision.Decisions.IsPlayerOutOfMoves, awaitingTurnState),
        });

        AnyState.AddTransitions(new[] {
            CreateEventAwaitTransition(PlayerEventType.MouseInputRequested, PlayerEventType.MouseInputAcquired),
            CreateEventAwaitTransition(PlayerEventType.StartedAnimation, PlayerEventType.EndedAnimation),
            new PlayerStateTransition(PlayerDecision.Decisions.DidCombatEnd, exploreState),
        });

        FirstState = exploreState;
        CurrentState = FirstState;
    }

    public bool CanPerformAction(DungeonActionType actionType)
    {
        return CanPerformActionInState(actionType);
    }

    private PlayerStateTransition CreateEventAwaitTransition(PlayerEventType triggerEvent, PlayerEventType awaitedEvent)
    {
        return new PlayerStateTransition(PlayerDecision.Decisions.DidEventOccur(triggerEvent), new PlayerAwaitingEventsState(awaitedEvent, this));
    }
}