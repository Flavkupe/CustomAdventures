using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface ITransition<TChangeContext>
{
    IDecision<TChangeContext> Decision { get; }
    IState<TChangeContext> Next { get; }
}

public class Transition<TChangeContext> : ITransition<TChangeContext>
{
    public Transition(IDecision<TChangeContext> decision, IState<TChangeContext> next)
    {
        Decision = decision;
        Next = next;
    }

    public IDecision<TChangeContext> Decision { get; }
    public IState<TChangeContext> Next { get; }
}

/// <summary>
/// A transition that goes back to the previous state, whatever it was
/// </summary>
public class ReturnTransition<TStateType, TChangeContext> : ITransition<TChangeContext> where TStateType : class, IState<TChangeContext>
{
    private readonly StateController<TStateType, TChangeContext> _controller;
    public ReturnTransition(IDecision<TChangeContext> decision, StateController<TStateType, TChangeContext> controller)
    {
        _controller = controller;
        Decision = decision;
    }

    public IDecision<TChangeContext> Decision { get; }
    public IState<TChangeContext> Next => _controller.PreviousState;
}