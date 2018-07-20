using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class AwaitingInputState : DungeonState
{
    public AwaitingInputState()
    {
    }

    public AwaitingInputState(IEnumerable<DungeonStateTransition> transitions) : base(transitions)
    {
    }

    public override void StateEntered(IState<DungeonStateChangeContext> previousState, DungeonStateChangeContext context)
    {
    }

    public override void StateExited(IState<DungeonStateChangeContext> newState, DungeonStateChangeContext context)
    {
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

    protected override void HandleEvent(DungeonStateChangeContext context)
    {
    }

    public override void Update(GameContext context)
    {
    }
}

