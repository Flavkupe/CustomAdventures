using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player.State.Context;
using UnityEngine;

public class PlayerCombatTurnState : PlayerState
{
    public PlayerCombatTurnState(StateController<PlayerStateChangeContext> controller) : base(controller)
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

    protected override bool HandleInput(GameContext context)
    {
        if (base.HandleInput(context))
        {
            return true;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            context.Player.CurrentStats.FreeMoves = 0;
            context.Player.CurrentStats.FullActions = 0;
            OnAfterPlayerMove(context);
            return true;
        }

        return false;
    }

    protected override bool OnDirectionInput(Direction direction, GameContext context)
    {
        if (TryMoveToDirection(direction, context))
        {
            return true;
        }

        var player = context.Player;
        if (player.PlayerHasActions)
        {
            if (TryInteractWithDirection(direction, context))
            {
                return true;
            }
        }

        return false;
    }

    protected override IEnumerator OnBeforePlayerMove(GameContext context)
    {
        yield return base.OnBeforePlayerMove(context);
        var adjacentEnemies = context.Dungeon.GetEntitiesNearPlayer(TileRangeType.Sides, 1, TileEntityType.Enemy);
        yield return ActivateOpportunityAttacks(adjacentEnemies.OfType<Enemy>());
    }

    private IEnumerator ActivateOpportunityAttacks(IEnumerable<Enemy> enemies)
    {
        foreach (var enemy in enemies)
        {
            yield return enemy.AttackPlayer();
        }
    }

    protected override void OnAfterPlayerAction(GameContext context, bool isFullAction)
    {
        if (!isFullAction && context.Player.CurrentStats.FreeMoves > 0)
        {
            context.Player.CurrentStats.FreeMoves--;
        }
        else
        {
            context.Player.CurrentStats.FullActions--;
            context.Player.CurrentStats.FreeMoves = 0;
        }

        context.Player.ActionTaken();
    }
}