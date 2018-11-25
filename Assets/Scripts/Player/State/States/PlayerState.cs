using System.Collections;
using UnityEngine;
using Assets.Scripts.State;
using System;

public class PlayerStateTransition : Transition<PlayerEventType>
{
    public PlayerStateTransition(IDecision<PlayerEventType> decision, IState<PlayerEventType> next)
        : base(decision, next)
    {
    }
}

public class PlayerReturnStateTransition : ReturnTransition<PlayerState, PlayerEventType>
{
    public PlayerReturnStateTransition(IDecision<PlayerEventType> decision, PlayerStateController controller)
        : base(decision, controller)
    {
    }
}

public abstract class PlayerState : State<PlayerEventType>, IActionDeterminant<DungeonActionType>, IHandleStateEvent<DungeonEventType>
{
    protected PlayerState(IStateController<PlayerEventType> controller): base(controller)
    {
    }

    private bool _moving;

    public abstract bool CanPerformAction(DungeonActionType actionType);

    protected void RaiseEventOccurred(PlayerEventType newEvent, GameContext context)
    {
        RaiseEventOccurred(new StateContext<PlayerEventType>(newEvent, context, this));
    }

    public sealed override void Update(GameContext context)
    {
        HandleInput(context);
    }

    protected virtual bool HandleInput(GameContext context)
    {
        var moved = false;
        if (_moving || context.Dungeon.IsGamePaused || Game.UI.IsMenuActive)
        {
            return true;
        }

        if (!context.CanPerformAction(DungeonActionType.PlayerMove))
        {
            return true;
        }

        if (Input.GetKey(KeyCode.W))
        {
            moved = OnDirectionInput(Direction.Up, context);            
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moved = OnDirectionInput(Direction.Down, context);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moved = OnDirectionInput(Direction.Left, context);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moved = OnDirectionInput(Direction.Right, context);
        }

        if (moved)
        {
            context.UI.UpdateEntityPanels();
        }

        return moved;
    }

    protected bool TryMoveToDirection(Direction direction, GameContext context)
    {
        var player = context.Player;
        if (direction == Direction.Left || direction == Direction.Right)
        {
            player.FaceDirection(direction);
        }

        if (!player.CanMove(direction))
        {
            return false;
        }

        var moveRoutine = Routine.Create(TryMovePlayerEntity, direction, context);
        moveRoutine.Then(() => _moving = false);
        _moving = true;
        EnqueueRoutine(moveRoutine);
        return true;
    }

    protected bool TryInteractWithDirection(Direction direction, GameContext context)
    {
        var player = context.Player;
        var obj = context.Dungeon.Grid.GetAdjacentObject(player.XCoord, player.YCoord, direction);
        if (obj != null)
        {
            if (obj.PlayerCanInteractWith())
            {
                PlayerInteractWith(context, obj);
                return true;
            }
        }
        else
        {
            // Boundry
        }

        return false;
    }

    protected virtual bool OnDirectionInput(Direction direction, GameContext context)
    {
        return false;
    }

    private IEnumerator TryMovePlayerEntity(Direction direction, GameContext context)
    {
        yield return OnBeforePlayerMove(context);
        yield return context.Player.TryMove(direction);
        OnAfterPlayerMove(context);
    }

    protected virtual IEnumerator OnBeforePlayerMove(GameContext context)
    {
        yield return null;
    }

    protected virtual void OnAfterPlayerInteract(GameContext context)
    {
        context.Player.ProcessEffects(EffectActivatorType.Steps);
        OnAfterPlayerAction(context, PlayerActionRequirementType.FullTurn);
    }

    protected virtual void OnAfterPlayerMove(GameContext context)
    {
        context.Player.ProcessEffects(EffectActivatorType.Steps);
        OnAfterPlayerAction(context, PlayerActionRequirementType.FreeMove);
    }

    protected virtual void OnAfterPlayerAttack(GameContext context)
    {
        context.Player.ProcessEffects(EffectActivatorType.Attacks);
        var inv = context.Player.Inventory;
        if (inv.IsSlotOccupied(InventoryItemType.Weapon))
        {
            inv.EquippedWeapon.ItemDurabilityExpended();
        }

        OnAfterPlayerAction(context, PlayerActionRequirementType.FullTurn);
    }

    protected virtual void OnAfterPlayerAction(GameContext context, PlayerActionRequirementType actionRequirement)
    {
    }

    protected void PlayerInteractWith(GameContext context, TileEntity obj)
    {
        var player = context.Player;
        var interaction = obj.GetPlayerInteraction(player);
        var interactionRoutine = Routine.Create(obj.PlayerInteractWith, player);

        bool doesTwitch = false;
        if (interaction == PlayerInteraction.Attack)
        {
            doesTwitch = true;
            player.PlayAttackEffects();
            interactionRoutine.Then(() => OnAfterPlayerAttack(context));
        }
        else if (interaction == PlayerInteraction.InteractWithObject)
        {
            doesTwitch = true;
            interactionRoutine.Then(() => OnAfterPlayerInteract(context));
        }

        Routine fullInteractionRoutine;
        if (doesTwitch)
        {
            fullInteractionRoutine = Routine.Create(player.TwitchTowards, obj, 5.0f);
            fullInteractionRoutine.Then(interactionRoutine);
        }
        else
        {
            fullInteractionRoutine = interactionRoutine;
        }

        RaiseEventOccurred(PlayerEventType.StartedAnimation, context);
        fullInteractionRoutine.Finally(() => RaiseEventOccurred(PlayerEventType.EndedAnimation, context));
        EnqueueRoutine(fullInteractionRoutine);
    }

    /// <summary>
    /// Handle events from Dungeon
    /// </summary>
    public virtual void HandleNewEvent(DungeonEventType eventType, GameContext context)
    {
        if (eventType == DungeonEventType.AllEnemiesTurnEnd)
        {
            RaiseEventOccurred(PlayerEventType.AITurnsComplete, context);
        }
    }
}

public enum PlayerActionRequirementType
{
    FullAction,
    FullTurn,
    FreeMove,
    Free,
}