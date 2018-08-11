﻿using System;
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

public abstract class DungeonState : State<DungeonStateChangeContext>, IActionDeterminant<DungeonActionType>
{
    public DungeonState(StateController<DungeonStateChangeContext> contoller) : base(contoller)
    {
    }

    public abstract bool CanPerformAction(DungeonActionType actionType);

    protected void RaiseEventOccurred(DungeonEventType newEvent, GameContext context)
    {
        RaiseEventOccurred(new DungeonStateChangeContext(newEvent, context));
    }
}
