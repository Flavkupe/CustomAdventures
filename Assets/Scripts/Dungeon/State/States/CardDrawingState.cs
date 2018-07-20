using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CardDrawingState : DungeonState
{
    public CardDrawingState()
    {
    }

    public CardDrawingState(IEnumerable<DungeonStateTransition> transitions) : base(transitions)
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
            case DungeonActionType.EntityMove:
            case DungeonActionType.UseItem:
                return false;
            default:
                return true;
        }
    }

    protected override void HandleEvent(DungeonStateChangeContext context)
    {
    }

    public override void Update(GameContext context)
    {
    }
}

