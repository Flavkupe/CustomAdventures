using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDecision<in TChangeContext>
{
    bool Evaluate(TChangeContext context);
}

public abstract class Decision<TChangeContext> : IDecision<TChangeContext>
{
    public abstract bool Evaluate(TChangeContext context);
}

