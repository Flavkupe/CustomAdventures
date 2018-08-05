using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DungeonStateController : StateController<DungeonStateChangeContext>, IActionDeterminant<DungeonActionType>
{
    public DungeonStateController() : base("Dungeon")
    {
        var awaitingInputState = new AwaitingInputState(this);
        var awaitingAIState = new AwaitingAIState(this);

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
