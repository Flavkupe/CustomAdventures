using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IState
{
    ITransition[] Transitions { get; }
}

public abstract class State
{
    public abstract ITransition[] Transitions { get; }
}
