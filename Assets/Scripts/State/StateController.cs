using UnityEngine;

public interface IStateController
{
    string Name { get; }
    string StateName { get; }
}

public interface IStateController<TContextType> : IStateController
{
    void Update(GameContext context);

    void RegisterState(IState<TContextType> state);
}

public abstract class StateController<TStateType, TContextType> : IStateController<TContextType> where TStateType : class, IState<TContextType>
{
    public TStateType CurrentState { get; protected set; }
    public TStateType PreviousState => _previous ?? CurrentState;

    private TStateType _previous = null;
    
    protected TStateType FirstState { get; set; }

    protected State<TContextType> AnyState { get; }

    private StateEventQueue _eventQueue;

    private readonly string _name;

    public bool QueueIdle => _eventQueue.Idle;

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
        AnyState = new State<TContextType>(this);
    }

    protected void ChangeState(TStateType newState, TContextType context)
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

    protected void CheckState(TContextType context)
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

    public void Update(GameContext context)
    {
        CurrentState.Update(context);
    }

    public void RegisterState(IState<TContextType> state)
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

    protected void EventOccurred(TContextType eventContext)
    {
        CurrentState.HandleNewEvent(eventContext);
        CheckState(eventContext);
    }
}