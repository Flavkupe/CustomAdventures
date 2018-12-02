using Assets.Scripts.State;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IStateController
{
    string Name { get; }
    string StateName { get; }

    /// <summary>
    /// For handling incoming events not related to this state
    /// </summary>
    void HandleNewEvent<TIncomingEventType>(TIncomingEventType eventType, GameContext context) where TIncomingEventType : struct;
}

public interface IStateController<TEventType> : IStateController where TEventType : struct
{
    void Update(GameContext context);

    void RegisterState(IState<TEventType> state);

    bool QueueIdle { get; }

    /// <summary>
    /// For handling incoming events
    /// </summary>
    void HandleNewEvent(TEventType eventType, GameContext context);

    event EventHandler<TEventType> NewEventRaised;
}

public abstract class StateController<TStateType, TEventType> : IStateController<TEventType> where TStateType : class, IState<TEventType> where TEventType : struct
{
    public TStateType CurrentState { get; protected set; }
    public TStateType PreviousState => _previous ?? CurrentState;

    private TStateType _previous = null;
    
    protected TStateType FirstState { get; set; }

    protected State<TEventType> AnyState { get; }

    private StateEventQueue _eventQueue;

    private readonly string _name;

    public bool QueueIdle => _eventQueue.Idle;

    public event EventHandler<TEventType> NewEventRaised;

    public string Name => _name;
    public string StateName => CurrentState.GetType().Name;

    public void Start()
    {
        var obj = new GameObject(_name);
        _eventQueue = obj.AddComponent<StateEventQueue>();
        Debug.Log($"{_name} Starting with state {CurrentState.GetType().Name}");
    }

    protected StateController(string subjectName)
    {
        _name = subjectName;
        AnyState = new State<TEventType>(this);
    }

    protected void ChangeState(TStateType newState, StateContext context)
    {
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

    protected void CheckState(StateContext<TEventType> context)
    {
        var newState = CurrentState.GetNextState(context);
        if (!(newState is TStateType))
        {
            Debug.LogError("State of wrong type used!");
            return;
        }

        if (newState == AnyState)
        {
            Debug.LogError("Can't transition into AnyState!");
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

        ChangeState((TStateType)newState, context);
    }

    protected void CheckState(GameContext context)
    {
        var newState = CurrentState.GetNextState(context);
        if (!(newState is TStateType))
        {
            Debug.LogError("State of wrong type used!");
            return;
        }

        if (newState == AnyState)
        {
            Debug.LogError("Can't transition into AnyState!");
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

        ChangeState((TStateType)newState, new StateContext(context));
    }

    public void Update(GameContext context)
    {
        CurrentState.Update(context);
    }

    public void RegisterState(IState<TEventType> state)
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
        EnqueueRoutine(routine);
    }

    protected void EnqueueRoutine(Routine routine)
    {
        if (routine != null)
        {
            _eventQueue.Enqueue(routine);
        }
    }

    protected void EventOccurred(StateContext<TEventType> eventContext)
    {
        CurrentState.HandleNewEvent(eventContext.Event, eventContext.GameContext);
        CheckState(eventContext);
    }

    /// <summary>
    /// For handling events related to this state
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="context"></param>
    public void HandleNewEvent(TEventType eventType, GameContext context)
    {
        NewEventRaised?.Invoke(this, eventType);
        EventOccurred(new StateContext<TEventType>(eventType, context, CurrentState));
    }

    /// <summary>
    /// For handling events not related to this state
    /// </summary>
    public void HandleNewEvent<TIncomingEventType>(TIncomingEventType eventType, GameContext context) where TIncomingEventType : struct
    {
        // Do not need to handle these
        if (typeof(TIncomingEventType) == typeof(TEventType))
        {
            return;
        }

        var state = CurrentState as IHandleStateEvent<TIncomingEventType>;
        if (state != null)
        {
            state.HandleNewEvent(eventType, context);
        }

        // Always attempt to update on any new event
        CheckState(context);
    }
}