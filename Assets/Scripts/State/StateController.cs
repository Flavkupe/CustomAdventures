using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    protected bool CanPerformActionInState<TActionType>(TActionType actionType)
    {
        var check = CurrentState as IActionDeterminant<TActionType>;
        if (check != null)
        {
            return check.CanPerformAction(actionType);
        }

        return false;
    }

    protected void EventOccurred(TContextType eventContext)
    {
        CurrentState.EventOccurred(eventContext);
        CheckState(eventContext);
    }
}