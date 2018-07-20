using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DungeonStateChangeContext
{
    public DungeonStateChangeContext(DungeonEventType dungeonEvent, GameContext gameContext)
    {
        EventType = dungeonEvent;
        GameContext = gameContext;
    }

    public DungeonEventType EventType { get; }

    public GameContext GameContext { get; }
}

