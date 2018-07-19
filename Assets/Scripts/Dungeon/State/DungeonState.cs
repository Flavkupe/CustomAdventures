using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class DungeonState : State
{
    public abstract bool CanPerformAction(DungeonActionType actionType);

    public void EventOccurred(DungeonEventType eventType, GameContext context)
    {
        // TODO: can we make these states generic but still useful?   
    }
}

