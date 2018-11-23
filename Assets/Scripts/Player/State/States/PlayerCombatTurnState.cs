﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.State;

/// <summary>
/// State in which it's the player's turn in combat and some decision is being awaited
/// </summary>
public class PlayerCombatTurnState : PlayerState
{
    public PlayerCombatTurnState(IStateController<PlayerEventType> controller) : base(controller)
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

    public override void StateEntered(IState<PlayerEventType> previousState, StateContext<PlayerEventType> context)
    {
        context.GameContext.Player.InitializePlayerTurn();
        Game.UI.UIEventTriggered += UIEventTriggered;
        Game.UI.ActionIcons.ToggleIcons(true);
        Game.UI.ToggleCombatActionPanel(true);
    }

    public override void StateExited(IState<PlayerEventType> newState, StateContext<PlayerEventType> context)
    {
        Game.UI.ActionIcons.ToggleIcons(false);
        Game.UI.UIEventTriggered -= UIEventTriggered;
        Game.UI.ToggleCombatActionPanel(false);
    }

    protected override bool HandleInput(GameContext context)
    {
        if (base.HandleInput(context))
        {
            return true;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            context.Player.CurrentStats.FreeMoves.Value = 0;
            context.Player.CurrentStats.FullActions.Value = 0;
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

    private void UIEventTriggered(object sender, UIEvent e)
    {
        if (e == UIEvent.SkipTurnPressed)
        {
            SkipTurn();
        }
    }

    private void SkipTurn()
    {
        var context = Game.Dungeon.GetGameContext();
        context.Player.CurrentStats.FreeMoves.Value = 0;
        context.Player.CurrentStats.FullActions.Value = 0;
        OnAfterPlayerMove(context);
    }

    protected override void OnAfterPlayerAction(GameContext context, bool isFullAction)
    {
        if (!isFullAction && context.Player.CurrentStats.FreeMoves > 0)
        {
            context.Player.CurrentStats.FreeMoves.Value--;
        }
        else
        {
            context.Player.CurrentStats.FullActions.Value--;
            context.Player.CurrentStats.FreeMoves.Value = 0;
        }

        context.Player.ActionTaken();
        RaiseEventOccurred(PlayerEventType.AfterMove, context);
    }
}