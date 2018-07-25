﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class AwaitingInputState : DungeonState
{
    public override bool CanPerformAction(DungeonActionType actionType)
    {
        switch (actionType)
        {
            case DungeonActionType.PlayerMove:
            case DungeonActionType.UseItem:
                return true;
            case DungeonActionType.EntityMove:
            case DungeonActionType.OpenMenu:
            default:
                return false;
        }
    }
}

