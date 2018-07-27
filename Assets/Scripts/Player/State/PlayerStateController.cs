using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Player.State.Context;

public class PlayerStateController : StateController<PlayerStateChangeContext>, IActionDeterminant<DungeonActionType>
{
    public PlayerStateController()
    {
        var exploreState = new PlayerExploreState();
        var awaitingTurnState = new PlayerAwaitingTurnState();
        var combatTurnState = new PlayerCombatTurnState();

        exploreState.AddTransitions(new[]
        {
            new PlayerStateTransition(PlayerDecision.Decisions.DidPlayerTurnStart, combatTurnState)
        });

        awaitingTurnState.AddTransitions(new[]
        {
            new PlayerStateTransition(PlayerDecision.Decisions.DidCombatEnd, exploreState),
            new PlayerStateTransition(PlayerDecision.Decisions.DidPlayerTurnStart, combatTurnState)
        });

        combatTurnState.AddTransitions(new[]
        {
            new PlayerStateTransition(PlayerDecision.Decisions.DidCombatEnd, exploreState),
            new PlayerStateTransition(PlayerDecision.Decisions.DidPlayerTurnEnd, awaitingTurnState)
        });

        FirstState = exploreState;
        CurrentState = FirstState;

        // Don't forget to register states...!
        RegisterStates(exploreState, awaitingTurnState, combatTurnState);
    }

    public void SendEvent(PlayerEventType eventType, GameContext context)
    {
        var eventContext = new PlayerStateChangeContext(eventType, context);
        EventOccurred(eventContext);
    }

    public bool CanPerformAction(DungeonActionType actionType)
    {
        return CanPerformActionInState(actionType);
    }
}