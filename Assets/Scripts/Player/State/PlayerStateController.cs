using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Player.State.Context;

public class PlayerStateController : StateController<PlayerStateChangeContext>, IActionDeterminant<DungeonActionType>
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
            new PlayerStateTransition(PlayerDecision.Decisions.DidCombatEnd, exploreState),
            new PlayerStateTransition(PlayerDecision.Decisions.DidPlayerTurnStart, combatTurnState)
        });

        combatTurnState.AddTransitions(new[]
        {
            new PlayerStateTransition(PlayerDecision.Decisions.DidCombatEnd, exploreState),
            new PlayerStateTransition(PlayerDecision.Decisions.DidPlayerTurnEnd, awaitingTurnState),
        });

        AnyState.AddTransitions(new[] {
            CreateEventAwaitTransition(PlayerEventType.MouseInputRequested, PlayerEventType.MouseInputAcquired),
            CreateEventAwaitTransition(PlayerEventType.StartedAnimation, PlayerEventType.EndedAnimation),
        });

        FirstState = exploreState;
        CurrentState = FirstState;
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

    private PlayerStateTransition CreateEventAwaitTransition(PlayerEventType triggerEvent, PlayerEventType awaitedEvent)
    {
        return new PlayerStateTransition(PlayerDecision.Decisions.DidEventOccur(triggerEvent), new PlayerAwaitingEventsState(awaitedEvent, this));
    }
}