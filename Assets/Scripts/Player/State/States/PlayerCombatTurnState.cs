using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player.State.Context;
using UnityEngine;

/// <summary>
/// State in which it's the player's turn in combat and some decision is being awaited
/// </summary>
public class PlayerCombatTurnState : PlayerState
{
    public PlayerCombatTurnState(IStateController<PlayerStateChangeContext> controller) : base(controller)
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

    public override void StateEntered(IState<PlayerStateChangeContext> previousState, PlayerStateChangeContext context)
    {
        context.GameContext.Player.InitializePlayerTurn();
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
        var player = context.Player;
        if (player.PlayerHasMoves && TryMoveToDirection(direction, context))
        {
            return true;
        }

        if (player.PlayerHasActions && TryInteractWithDirection(direction, context))
        {
            return true;
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