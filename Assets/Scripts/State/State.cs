using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IState<TChangeContext>
{
    IState<TChangeContext> GetNextState(TChangeContext context);

    void StateEntered(IState<TChangeContext> previousState, TChangeContext context);

    void StateExited(IState<TChangeContext> newState, TChangeContext context);

    void EventOccurred(TChangeContext context);

    void Update(GameContext context);
}

public abstract class State<TChangeContext> : IState<TChangeContext>
{
    protected State()
    {
        Transitions = new List<ITransition<TChangeContext>>();
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

    public virtual void EventOccurred(TChangeContext context)
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
