using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface ITransition
{
    IDecision Decision { get; }
    IState Next { get; }
}

public class Transition : ITransition
{
    public Transition(IDecision decision, IState next)
    {
        Decision = decision;
        Next = next;
    }

    public IDecision Decision { get; }
    public IState Next { get; }
}
