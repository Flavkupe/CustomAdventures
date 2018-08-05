using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class StateController<TContextType>
{
    public IState<TContextType> CurrentState { get; protected set; }
    public IState<TContextType> PreviousState => _previous ?? CurrentState;

    private IState<TContextType> _previous = null;
    
    protected IState<TContextType> FirstState { get; set; }

    protected IState<TContextType> AnyState { get; }

    private StateEventQueue _eventQueue;

    private string _name;

    public void Start()
    {
        var obj = new GameObject(_name);
        _eventQueue = obj.AddComponent<StateEventQueue>();
        Debug.Log($"{_name} Starting with state {CurrentState.GetType().Name}");
    }

    protected StateController(string subjectName)
    {
        _name = subjectName;
        AnyState = new State<TContextType>(this);
    }

    protected void CheckState(TContextType context)
    {
        var newState = CurrentState.GetNextState(context);

        if (newState == AnyState)
        {
            Debug.LogError($"Can't transition into AnyState!");
            return;
        }

        if (newState == CurrentState)
        {
            // If nothing changes, check global rules based on AnyState
            var state = AnyState.GetNextState(context);
            if (state != null && state != AnyState)
            {
                newState = state;
            }
        }

        if (newState != CurrentState)
        {
            Debug.Log($"{_name} Exiting state {CurrentState.GetType().Name}");
            Debug.Log($"{_name} Entering state {newState.GetType().Name}");
            _previous = CurrentState;
            CurrentState.StateExited(newState, context);
            newState.StateEntered(CurrentState, context);
            CurrentState = newState;
        }
    }

    public void Update(GameContext context)
    {
        CurrentState.Update(context);
    }

    public void RegisterState(State<TContextType> state)
    {
        state.EventOccurred += (obj, type) => EventOccurred(type);
        state.RequestRoutine += OnRequestRoutine;
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

    private void OnRequestRoutine(object sender, Routine routine)
    {
        if (routine != null)
        {
            _eventQueue.Enqueue(routine);
        }
    }

    protected void EventOccurred(TContextType eventContext)
    {
        CurrentState.HandleNewEvent(eventContext);
        CheckState(eventContext);
    }
}