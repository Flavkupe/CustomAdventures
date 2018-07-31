using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IState<TChangeContext>
{
    IState<TChangeContext> GetNextState(TChangeContext context);

    void StateEntered(IState<TChangeContext> previousState, TChangeContext context);

    void StateExited(IState<TChangeContext> newState, TChangeContext context);

    void HandleNewEvent(TChangeContext context);

    void Update(GameContext context);
}

public abstract class State<TChangeContext> : IState<TChangeContext>
{
    public event EventHandler<TChangeContext> EventOccurred;
    public event EventHandler<Routine> RequestRoutine;

    protected State()
    {
        Transitions = new List<ITransition<TChangeContext>>();
    }

    protected void RaiseEventOccurred(TChangeContext context)
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

    public void AddTransition(ITransition<TChangeContext> transition)
    {
        Transitions.Add(transition);
    }

    public void AddTransitions(IEnumerable<ITransition<TChangeContext>> transitions)
    {
        Transitions.AddRange(transitions);
    }

    public virtual void Update(GameContext context)
    {        
    }

    public virtual void HandleNewEvent(TChangeContext context)
    {
    }

    protected List<ITransition<TChangeContext>> Transitions { get; }

    public IState<TChangeContext> GetNextState(TChangeContext context)
    {
        foreach (var transition in Transitions)
        {
            if (transition.Decision.Evaluate(context))
            {
                return transition.Next;
            }
        }

        return this;
    }

    public virtual void StateEntered(IState<TChangeContext> previousState, TChangeContext context)
    {
    }

    public virtual void StateExited(IState<TChangeContext> newState, TChangeContext context)
    {
    }
}
