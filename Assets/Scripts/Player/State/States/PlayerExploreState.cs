/// <summary>
/// State in which the player is exploring the dungeon and can take inputs
/// </summary>
public class PlayerExploreState : PlayerState
{
    public PlayerExploreState(IStateController<PlayerEventType> controller) : base(controller)
    {
    }

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

    protected override void OnAfterPlayerAction(GameContext context, PlayerActionRequirementType actionRequirement)
    {
        base.OnAfterPlayerAction(context, actionRequirement);
        // context.Player.ActionTaken();
        // RaiseEventOccurred(PlayerEventType.AfterMove, context);
        context.Dungeon.BroadcastEvent(PlayerEventType.AfterMove);
    }

    protected override bool OnDirectionInput(Direction direction, GameContext context)
    {
        if (TryMoveToDirection(direction, context))
        {
            return true;
        }

        if (TryInteractWithDirection(direction, context))
        {
            return true;
        }

        return false;
    }
}