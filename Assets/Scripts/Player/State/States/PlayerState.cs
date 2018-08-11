
using System.Collections;
using System.Linq;
using Assets.Scripts.Player.State.Context;
using UnityEngine;

public class PlayerStateTransition : Transition<PlayerStateChangeContext>
{
    public PlayerStateTransition(IDecision<PlayerStateChangeContext> decision, IState<PlayerStateChangeContext> next)
        : base(decision, next)
    {
    }
}

public class PlayerReturnStateTransition : ReturnTransition<PlayerStateChangeContext>
{
    public PlayerReturnStateTransition(IDecision<PlayerStateChangeContext> decision, StateController<PlayerStateChangeContext> controller)
        : base(decision, controller)
    {
    }
}

public abstract class PlayerState : State<PlayerStateChangeContext>, IActionDeterminant<DungeonActionType>
{
    public PlayerState(StateController<PlayerStateChangeContext> controller): base(controller)
    {
    }

    private bool _moving;

    public abstract bool CanPerformAction(DungeonActionType actionType);

    protected void RaiseEventOccurred(PlayerEventType newEvent, DungeonStateChangeContext context)
    {
        RaiseEventOccurred(new PlayerStateChangeContext(newEvent, context.GameContext));
    }

    public sealed override void Update(GameContext context)
    {
        HandleInput(context);
    }

    protected virtual bool HandleInput(GameContext context)
    {
        var moved = false;
        if (_moving || context.Dungeon.IsGamePaused || Game.States.AreMenusOpen)
        {
            return true;
        }

        if (!context.DungeonActionChecks.CanPerformAction(DungeonActionType.PlayerMove))
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

        if (player.PlayerHasMoves && player.CanMove(direction))
        {
            // Set state to ensure we don't queue multiple moves
            Game.States.SetState(GameState.CharacterMoving);
            var moveRoutine = Routine.Create(TryMovePlayerEntity, direction, context);
            moveRoutine.Then(() => _moving = false);
            _moving = true;
            EnqueueRoutine(moveRoutine);
            return true;
        }

        return false;
    }

    protected bool TryInteractWithDirection(Direction direction, GameContext context)
    {
        var player = context.Player;
        var obj = context.Dungeon.Grid.GetAdjacentObject(player.XCoord, player.YCoord, direction);
        if (obj != null)
        {
            if (obj.PlayerCanInteractWith())
            {
                Game.States.SetState(GameState.CharacterActing);
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
        OnAfterPlayerAction(context, true);
    }

    protected virtual void OnAfterPlayerMove(GameContext context)
    {
        context.Player.ProcessEffects(EffectActivatorType.Steps);
        OnAfterPlayerAction(context, false);
    }

    protected virtual void OnAfterPlayerAttack(GameContext context)
    {
        context.Player.ProcessEffects(EffectActivatorType.Attacks);
        var inv = context.Player.Inventory;
        if (inv.IsSlotOccupied(InventoryItemType.Weapon))
        {
            inv.EquippedWeapon.ItemDurabilityExpended();
        }

        OnAfterPlayerAction(context, true);
    }

    protected virtual void OnAfterPlayerAction(GameContext context, bool isFullAction)
    {
    }

    protected void PlayerInteractWith(GameContext context, TileEntity obj)
    {
        var player = context.Player;
        var interaction = obj.GetPlayerInteraction(player);
        var routine = Routine.Create(obj.PlayerInteractWith, player);

        if (interaction == PlayerInteraction.Attack)
        {
            player.PlayAttackEffects();
            routine.Then(() => OnAfterPlayerAttack(context));
        }
        else if (interaction == PlayerInteraction.InteractWithObject)
        {
            routine.Then(() => OnAfterPlayerInteract(context));
        }

        EnqueueRoutine(routine);
        // Game.States.EnqueueCoroutine(routine);
    }
}