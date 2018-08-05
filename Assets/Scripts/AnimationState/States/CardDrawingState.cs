using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CardDrawingState : AnimationState
{
    public CardDrawingState(StateController<AnimationStateChangeContext> contoller) : base(contoller)
    {
    }

    public override bool CanPerformAction(DungeonActionType actionType)
    {
        switch (actionType)
        {
            case DungeonActionType.PlayerMove:
            case DungeonActionType.EntityMove:
            case DungeonActionType.UseItem:
                return false;
            default:
                return true;
        }
    }
}