using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDecision
{
    bool Evaluate(GameContext context);
}

public abstract class Decision : IDecision
{
    public abstract bool Evaluate(GameContext context);
}

