using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class DungeonStateTransition : Transition<DungeonStateChangeContext>
{
    public DungeonStateTransition(IDecision<DungeonStateChangeContext> decision, IState<DungeonStateChangeContext> next) 
        : base(decision, next)
    {
    }
}


public abstract class DungeonState : State<DungeonStateChangeContext>
{
    protected DungeonState()
    {
    }

    protected DungeonState(IEnumerable<ITransition<DungeonStateChangeContext>> transitions): base (transitions) {}

    public abstract bool CanPerformAction(DungeonActionType actionType);

    public override void EventOccurred(DungeonStateChangeContext context)
    {
        HandleEvent(context);
    }

    public override IState<DungeonStateChangeContext> GetNextState(DungeonStateChangeContext context)
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

    protected abstract void HandleEvent(DungeonStateChangeContext context);
}
