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
