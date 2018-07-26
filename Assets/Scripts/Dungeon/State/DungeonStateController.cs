using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DungeonStateController : StateController<DungeonStateChangeContext>, IActionDeterminant<DungeonActionType>
{
    public DungeonStateController()
    {
        var awaitingInputState = new AwaitingInputState();
        var awaitingAIState = new AwaitingAIState();

        awaitingInputState.AddTransitions(new[]
        {
            new DungeonStateTransition(DungeonDecision.Decisions.DidEnemyTurnStart, awaitingAIState)
        });

        awaitingAIState.AddTransitions(new[]
        {
            new DungeonStateTransition(DungeonDecision.Decisions.DidEnemyTurnEnd, awaitingInputState)
        });

        FirstState = awaitingInputState;
        CurrentState = FirstState;

        RegisterStates(awaitingInputState, awaitingAIState);
    }

    public void SendEvent(DungeonEventType eventType, GameContext context)
    {
        var eventContext = new DungeonStateChangeContext(eventType, context);
        EventOccurred(eventContext);
    }

    public bool CanPerformAction(DungeonActionType actionType)
    {
        return CanPerformActionInState(actionType);
    }
}
