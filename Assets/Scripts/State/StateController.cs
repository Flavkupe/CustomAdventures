using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class StateController<TContextType>
{
    protected IState<TContextType> CurrentState { get; set; }

    protected IState<TContextType> FirstState { get; set; }

    protected void CheckState(TContextType context)
    {
        var newState = CurrentState.GetNextState(context);

        if (newState != CurrentState)
        {
            Debug.Log($"Exiting state {CurrentState.GetType().Name}");
            Debug.Log($"Entering state {newState.GetType().Name}");
            CurrentState.StateExited(newState, context);
            newState.StateEntered(CurrentState, context);
            CurrentState = newState;
        }
    }

    public void Update(GameContext context)
    {
        CurrentState.Update(context);
    }

    protected bool CanPerformActionInState<TActionType>(TActionType actionType)
    {
        var check = CurrentState as IActionDeterminant<TActionType>;
        if (check != null)
        {
            return check.CanPerformAction(actionType);
        }

        return false;
    }

    protected void RegisterStates(params State<TContextType>[] states)
    {
        foreach (var state in states)
        {
            state.EventOccurred += (obj, type) => EventOccurred(type);
        }
    }

    protected void EventOccurred(TContextType eventContext)
    {
        CurrentState.HandleNewEvent(eventContext);
        CheckState(eventContext);
    }
}