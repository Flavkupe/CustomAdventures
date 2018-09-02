using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface ITransition<TEventType> where TEventType : struct
{
    IDecision<TEventType> Decision { get; }
    IState<TEventType> Next { get; }
}

public class Transition<TEventType> : ITransition<TEventType> where TEventType : struct
{
    public Transition(IDecision<TEventType> decision, IState<TEventType> next)
    {
        Decision = decision;
        Next = next;
    }

    public IDecision<TEventType> Decision { get; }
    public IState<TEventType> Next { get; }
}

/// <summary>
/// A transition that goes back to the previous state, whatever it was
/// </summary>
public class ReturnTransition<TStateType, TEventType> : ITransition<TEventType> where TStateType : class, IState<TEventType> where TEventType : struct
{
    private readonly StateController<TStateType, TEventType> _controller;
    public ReturnTransition(IDecision<TEventType> decision, StateController<TStateType, TEventType> controller)
    {
        _controller = controller;
        Decision = decision;
    }

    public IDecision<TEventType> Decision { get; }
    public IState<TEventType> Next => _controller.PreviousState;
}