using Assets.Scripts.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IState<TEventType> : IHandleStateEvent<TEventType> where TEventType : struct
{
    event EventHandler<StateContext<TEventType>> EventOccurred;
    event EventHandler<Routine> RequestRoutine;

    IState<TEventType> GetNextState(StateContext<TEventType> context);

    IState<TEventType> GetNextState(GameContext context);

    void StateEntered(IState<TEventType> previousState, StateContext context);

    void StateExited(IState<TEventType> newState, StateContext context);

    void AddTransitions(IEnumerable<ITransition<TEventType>> transitions);

    void AddTransition(ITransition<TEventType> transition);

    void Update(GameContext context);

    IStateController<TEventType> Controller { get; }
}

public class State<TEventType> : IState<TEventType> where TEventType : struct
{
    public event EventHandler<StateContext<TEventType>> EventOccurred;
    public event EventHandler<Routine> RequestRoutine;
    public IStateController<TEventType> Controller { get; }

    public State(IStateController<TEventType> controller)
    {
        controller.RegisterState(this);
        Controller = controller;
        Transitions = new List<ITransition<TEventType>>();
    }

    protected void RaiseEventOccurred(StateContext<TEventType> context)
    {
        EventOccurred?.Invoke(this, context);
    }

    protected void EnqueueRoutine(Routine routine)
    {
        RequestRoutine?.Invoke(this, routine);
    }

    protected void EnqueueCoroutine(IEnumerator coroutine)
    {
        RequestRoutine?.Invoke(this, Routine.Create(coroutine));
    }

    public void AddTransition(ITransition<TEventType> transition)
    {
        Transitions.Add(transition);
    }

    public void AddTransitions(IEnumerable<ITransition<TEventType>> transitions)
    {
        Transitions.AddRange(transitions);
    }

    public virtual void Update(GameContext context)
    {        
    }

    public virtual void HandleNewEvent(TEventType eventType, GameContext context)
    {
    }

    protected List<ITransition<TEventType>> Transitions { get; }

    public IState<TEventType> GetNextState(StateContext<TEventType> context)
    {
        foreach (var transition in Transitions)
        {
            if (transition.Decision.EvaluateEvent(context) || transition.Decision.EvaluateContext(context.GameContext))
            {
                return transition.Next;
            }
        }

        return this;
    }

    public IState<TEventType> GetNextState(GameContext context)
    {
        foreach (var transition in Transitions)
        {
            if (transition.Decision.EvaluateContext(context))
            {
                return transition.Next;
            }
        }

        return this;
    }

    public virtual void StateEntered(IState<TEventType> previousState, StateContext context)
    {
    }

    public virtual void StateExited(IState<TEventType> newState, StateContext context)
    {
    }
}
