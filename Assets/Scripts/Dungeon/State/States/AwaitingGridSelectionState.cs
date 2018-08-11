using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Dungeon.State.Context;
using UnityEngine;

public class AwaitingGridSelectionState : DungeonState
{
    private int _numToSelect;
    private ActionOnEntities _doOnSelected;

    private EntitySelectionOptions _options;
    private readonly List<TileEntity> _selectedTargets = new List<TileEntity>();

    public AwaitingGridSelectionState(StateController<DungeonStateChangeContext> contoller) : base(contoller)
    {
    }

    public override void StateEntered(IState<DungeonStateChangeContext> previousState, DungeonStateChangeContext context)
    {
        _selectedTargets.Clear();
        base.StateEntered(previousState, context);
        context.GameContext.Dungeon.TileSelected += OnTileSelected;
    }

    public override void StateExited(IState<DungeonStateChangeContext> newState, DungeonStateChangeContext context)
    {
        base.StateExited(newState, context);
        context.GameContext.Dungeon.TileSelected -= OnTileSelected;
        _selectedTargets.Clear();
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

        if (_selectedTargets.Count >= _numToSelect)
        {
            // Perform event, then after events done, change state back
            var performOnSelected = Routine.Create(() => _doOnSelected(_selectedTargets));
            performOnSelected.Then(() => RaiseEventOccurred(DungeonEventType.SelectionCompleted, Game.Dungeon.GetGameContext()));
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
        if (Input.GetMouseButtonUp(1))
        {
            RaiseEventOccurred(DungeonEventType.SelectionCancelled, context);

        }
    }
}