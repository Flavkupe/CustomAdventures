using Assets.Scripts.Player.State.Context;

/// <summary>
/// State in which the player is await its turn, probably because the AI or enemy
/// is moving
/// </summary>
public class PlayerAwaitingTurnState : PlayerState
{
    public PlayerAwaitingTurnState(StateController<PlayerStateChangeContext> controller) : base(controller)
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
                return false;
        }
    }
}
