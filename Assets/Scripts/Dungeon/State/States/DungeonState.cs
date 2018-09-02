using Assets.Scripts.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DungeonStateTransition : Transition<DungeonEventType>
{
    public DungeonStateTransition(IDecision<DungeonEventType> decision, IState<DungeonEventType> next) 
        : base(decision, next)
    {
    }
}

public abstract class DungeonState : State<DungeonEventType>, IActionDeterminant<DungeonActionType>
{
    protected DungeonState(IStateController<DungeonEventType> controller) : base(controller)
    {
    }

    public abstract bool CanPerformAction(DungeonActionType actionType);

    new protected void RaiseEventOccurred(StateContext<DungeonEventType> context)
    {
        RaiseEventOccurred(context.Event, context.GameContext, true);
    }

    protected void RaiseEventOccurred(DungeonEventType newEvent, GameContext context, bool broadcast)
    {
        if (broadcast)
        {
            context.Dungeon.BroadcastEvent(newEvent);
        }
        else
        {
            base.RaiseEventOccurred(new StateContext<DungeonEventType>(newEvent, context, this));
        }
    }
}
