using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// State in which the player's input is being awaited, such as waiting for keypresses for movement
/// </summary>
public class AwaitingInputState : DungeonState
{
    public AwaitingInputState(IStateController<DungeonStateChangeContext> contoller) : base(contoller)
    {
    }

    public override void StateEntered(IState<DungeonStateChangeContext> previousState, DungeonStateChangeContext context)
    {
    }

    public override bool CanPerformAction(DungeonActionType actionType)
    {
        switch (actionType)
        {
            case DungeonActionType.PlayerMove:
            case DungeonActionType.UseItem:
            case DungeonActionType.PerformCardDraw:
                return true;
            case DungeonActionType.EntityMove:
            case DungeonActionType.OpenMenu:
            default:
                return false;
        }
    }
}