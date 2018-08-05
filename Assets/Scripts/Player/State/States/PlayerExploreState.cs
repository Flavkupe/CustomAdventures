using System.Collections;
using System.Linq;
using Assets.Scripts.Player.State.Context;
using UnityEngine;

public class PlayerExploreState : PlayerState
{
    public PlayerExploreState(StateController<PlayerStateChangeContext> controller) : base(controller)
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

    protected override void OnAfterPlayerAction(GameContext context, bool isFullAction)
    {
        base.OnAfterPlayerAction(context, isFullAction);
        context.Player.ActionTaken();
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