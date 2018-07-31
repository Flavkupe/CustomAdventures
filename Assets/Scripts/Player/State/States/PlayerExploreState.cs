using System.Collections;
using System.Linq;

public class PlayerExploreState : PlayerState
{
    public override bool CanPerformAction(DungeonActionType actionType)
    {
        switch (actionType)
        {
            case DungeonActionType.PlayerMove:
            case DungeonActionType.EntityMove:
            case DungeonActionType.OpenMenu:
            case DungeonActionType.UseItem:
            default:
                return true;
        }
    }

    protected override void OnAfterPlayerAction(GameContext context, bool isFullAction)
    {
        base.OnAfterPlayerAction(context, isFullAction);
        context.Player.ActionTaken();
    }
}