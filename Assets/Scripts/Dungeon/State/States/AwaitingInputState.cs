using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AwaitingInputState : DungeonState
{
    public override void StateEntered(IState<DungeonStateChangeContext> previousState, DungeonStateChangeContext context)
    {
        Game.States.SetState(GameState.AwaitingCommand);
        context.GameContext.Player.InitializePlayerTurn();
    }

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

    public override void HandleNewEvent(DungeonStateChangeContext context)
    {
        if (context.EventType == DungeonEventType.AfterPlayerTurn)
        {
            Game.States.SetState(GameState.AwaitingCommand);
        }
    }
}
