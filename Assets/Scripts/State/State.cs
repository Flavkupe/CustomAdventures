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

    protected State(IEnumerable<ITransition<TChangeContext>> transitions)
    {
        Transitions = transitions?.ToList() ?? new List<ITransition<TChangeContext>>();
    }

    public void AddTransition(ITransition<TChangeContext> transition)
    {
        Transitions.Add(transition);
    }

    public void AddTransitions(IEnumerable<ITransition<TChangeContext>> transitions)
    {
        Transitions.AddRange(transitions);
    }

    public abstract void Update(GameContext context);

    public abstract void EventOccurred(TChangeContext context);

    protected List<ITransition<TChangeContext>> Transitions { get; }

    public abstract IState<TChangeContext> GetNextState(TChangeContext context);

    public abstract void StateEntered(IState<TChangeContext> previousState, TChangeContext context);

    public abstract void StateExited(IState<TChangeContext> newState, TChangeContext context);
}
