using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Enum of possible actions that can be taken which could be controlled
/// by DungeonStateController
/// </summary>
public enum DungeonActionType
{
    PlayerMove,
    EntityMove,
    OpenMenu,
    UseItem,
}

// new DungeonStateTransition(new AreCardsDoneMovingDecision(), new AwaitingInputState())

public abstract class StateController<TContextType>
{
    protected IState<TContextType> CurrentState { get; set; }

    protected IState<TContextType> FirstState { get; set; }

    protected void CheckState(TContextType context)
    {
        var newState = CurrentState.GetNextState(context);
        
        if (newState != CurrentState)
        {
            CurrentState.StateExited(newState, context);
            newState.StateEntered(CurrentState, context);
            CurrentState = newState;
        }
    }

    public void Update(GameContext context)
    {
        CurrentState.Update(context);
    }

    protected void EventOccurred(TContextType eventContext)
    {
        CurrentState.EventOccurred(eventContext);
        CheckState(eventContext);
    }
}

public class DungeonStateController : StateController<DungeonStateChangeContext>
{
    public DungeonStateController()
    {
        var awaitingState = new AwaitingInputState();
        var cardDrawState = new CardDrawingState();

        awaitingState.AddTransitions(new[]
        {
            new DungeonStateTransition(DungeonDecision.Decisions.DidCardsStartMovingDecision, cardDrawState)
        });

        cardDrawState.AddTransitions(new[]
        {
            new DungeonStateTransition(DungeonDecision.Decisions.AreCardsDoneMovingDecision, awaitingState)
        });

        FirstState = awaitingState;
        CurrentState = FirstState;
    }

    public void SendEvent(DungeonEventType eventType, GameContext context)
    {
        var eventContext = new DungeonStateChangeContext(eventType, context);
        EventOccurred(eventContext);
    }
}
