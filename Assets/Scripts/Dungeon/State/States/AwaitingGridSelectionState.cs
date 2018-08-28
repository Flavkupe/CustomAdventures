using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State in which the player is making a selection on the grid
/// </summary>
public class AwaitingGridSelectionState : DungeonState
{
    private ActionOnEntities _doOnSelected;

    private EntitySelectionOptions _options;
    private readonly List<TileEntity> _selectedTargets = new List<TileEntity>();

    public AwaitingGridSelectionState(IStateController<DungeonStateChangeContext> contoller) : base(contoller)
    {
    }

    public override void StateEntered(IState<DungeonStateChangeContext> previousState, DungeonStateChangeContext context)
    {
        base.StateEntered(previousState, context);
        _selectedTargets.Clear();
        context.GameContext.Dungeon.Grid.UnselectAllTileEntities();
        context.GameContext.Dungeon.TileEntityClicked += OnTileSelected;
        _options.TileRange.ForEach(tile => tile.Show(true));
    }

    public override void StateExited(IState<DungeonStateChangeContext> newState, DungeonStateChangeContext context)
    {
        base.StateExited(newState, context);
        context.GameContext.Dungeon.TileEntityClicked -= OnTileSelected;
        _selectedTargets.Clear();
        context.GameContext.Dungeon.Grid.UnselectAllTileEntities();
        _options.TileRange.ForEach(tile => tile.Show(false));
    }

    private void OnTileSelected(object sender, TileEntity e)
    {
        if (_options.Filter.Count > 0 && !_options.Filter.Contains(e))
        {
            return;
        }

        e.Selected = !e.Selected;
        if (e.Selected && !_selectedTargets.Contains(e))
        {
            _selectedTargets.Add(e);
        }
        else if (!e.Selected && _selectedTargets.Contains(e))
        {
            _selectedTargets.Remove(e);
        }

        if (_selectedTargets.Count >= _options.NumToSelect)
        {
            // Perform event, then after events done, change state back
            var performOnSelected = Routine.Create(() => _doOnSelected(_selectedTargets));
            performOnSelected.Finally(() => RaiseEventOccurred(DungeonEventType.SelectionCompleted, Game.Dungeon.GetGameContext()));
            EnqueueRoutine(performOnSelected);
        }
    }

    public void StartSelection(GameContext context, EntitySelectionOptions options, ActionOnEntities doOnSelected)
    {
        _options = options;
        _doOnSelected = doOnSelected;
    }

    public override bool CanPerformAction(DungeonActionType actionType)
    {
        switch (actionType)
        {
            case DungeonActionType.PlayerMove:
            case DungeonActionType.UseItem:
            case DungeonActionType.EntityMove:
            case DungeonActionType.OpenMenu:
            default:
                return false;
        }
    }

    public override void Update(GameContext context)
    {
        base.Update(context);

        // Right-click to cancel
        if (Input.GetMouseButtonUp(1))
        {
            RaiseEventOccurred(DungeonEventType.SelectionCancelled, context);

        }
    }
}