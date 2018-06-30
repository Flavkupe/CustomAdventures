﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

public class DungeonManager : SingletonObject<DungeonManager>
{
    public DungeonPrefabTemplates Templates;

    public event EventHandler<List<Enemy>> EnemyListChanged;

    public event EventHandler AllEnemyTurnsComplete;

    /// <summary>
    /// Event that fires when list of enemies goes from 0 to more than 0.
    /// This indicates an event like combat starting.
    /// </summary>
    public event EventHandler EnemyListPopulated;

    public DungeonCardData[] PossibleBossCards;

    public AnimationEffectData CardTriggerEffect;
    public TargetedAnimationEffectData CardMoveToEffect;

    public GridTile GenericTileTemplate;
    public TileGrid Grid;
    public Room[,] RoomGrid;

    private List<Enemy> _enemies = new List<Enemy>();

    public void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
        Grid.ClearTileEntity(enemy.XCoord, enemy.YCoord);
        EnemyListChanged?.Invoke(this, _enemies);
    }

    public void RegisterEnemy(Enemy enemy)
    {
        var enemiesPopulated = _enemies.Count == 0;
        _enemies.Add(enemy);
        EnemyListChanged?.Invoke(this, _enemies);
        if (enemiesPopulated)
        {
            EnemyListPopulated?.Invoke(this, null);
        }
    }

    public bool IsCombat => _enemies.Count > 0;

    public void AfterPlayerTurn()
    {
        if (IsCombat && !Game.Player.PlayerCanMove)
        {
            Game.States.SetState(GameState.EnemyTurn);            
            RoutineChain enemyTurns = new RoutineChain(_enemies.Select(a => Routine.Create(a.ProcessCharacterTurn)).ToArray());
            enemyTurns.Then(() =>
            {
                AllEnemyTurnsComplete?.Invoke(this, null);
            });

            Game.States.EnqueueCoroutine(enemyTurns);
        }
        else if (!Game.States.AreMenusOpen)
        {
            Game.States.SetState(GameState.AwaitingCommand);
        }
    }

    private List<TileEntity> _validSelectionTargets;
    private List<TileEntity> _selectedTargets = new List<TileEntity>();
    public List<TileEntity> SelectedTargets { get { return _selectedTargets; } }
    public IEnumerator AwaitTargetSelection(Action cancellationCallback, List<TileEntity> entities, int numToSelect)
    {
        _selectedTargets.Clear();
        _validSelectionTargets = entities;
        //numToSelect = Math.Min(this._validSelectionTargets.Count, numToSelect);
        //if (numToSelect == 0)
        //{
        //    // Nothing available to select!
        //    cancellationCallback();
        //    yield break;
        //}

        Game.States.SetState(GameState.AwaitingSelection);

        while (_selectedTargets.Count < numToSelect)
        {
            if (Input.GetMouseButtonUp(1))
            {
                cancellationCallback();
                yield break;
            }

            yield return null;
        }

        foreach (TileEntity target in _selectedTargets)
        {
            // Reset selected state after all found
            target.Selected = false;
        }
    }

    public void AfterToggledSelection(TileEntity tileEntity)
    {
        if (Game.States.State != GameState.AwaitingSelection || !_validSelectionTargets.Contains(tileEntity))
        {
            tileEntity.Selected = false;
            return;
        }

        if (!tileEntity.Selected && _selectedTargets.Contains(tileEntity))
        {
            _selectedTargets.Remove(tileEntity);
        }
        else if (tileEntity.Selected && !_selectedTargets.Contains(tileEntity))
        {
            _selectedTargets.Add(tileEntity);
        }
    }

    /// <summary>
    /// Cleanup that happens after all events in a group happen (ie multiple cards).
    /// </summary>
    public void PostGroupEventCleanup()
    {
        Grid.UnreserveAll();
    }

    public Routine CreateSpawnEventRoutine(RoomArea roomArea, IDungeonCard card)
    {
        var routines = new RoutineChain();

        // Card trigger effect
        var cardTriggerEffect = CardTriggerEffect.CreateEffect();
        cardTriggerEffect.transform.position = card.Object.transform.position;
        card.ToggleHideCard(true);
        routines.AddRoutine(cardTriggerEffect.CreateRoutine());

        var numTimes = card.GetNumberOfExecutions();   

        var routineSet = new ParallelRoutineSet();
        for (var i = 0; i < numTimes; i++)
        {
            var tile = GetTargetTile(roomArea, card);

            if (tile != null)
            {
                // Card travel effect
                var cardTravelEffect = CardMoveToEffect.CreateTargetedEffect(tile.transform.position, card.Object.transform.position);
                var routine = cardTravelEffect.CreateRoutine();
                routine.Finally(() =>
                {
                    if (card.RequiresFullTile)
                    {
                        // TODO: I don't think the IsReserved system is needed....
                        tile.IsReserved = true;
                    }

                    var context = new DungeonCardExecutionContext()
                    {
                        Player = Game.Player,
                        Dungeon = this,
                    };

                    card.ExecuteTileSpawnEvent(tile, context);
                });

                routineSet.AddRoutine(routine);
            }
            else
            {
                // TODO: no tile chosen? What do?
            }
        }

        routines.AddRoutine(routineSet.AsRoutine());
        var finalRoutine = routines.ToRoutine();
        finalRoutine.Finally(card.DestroyCard);

        return finalRoutine;
    }

    private GridTile GetTargetTile(RoomArea roomArea, IDungeonCard card)
    {
        var grid = Grid;
        GridTile tile = null;
        if (card.DungeonEventType == DungeonEventType.SpawnNear)
        {
            var tiles = roomArea.GetAreaTiles();
            tile = tiles.Where(a => grid.CanOccupy(a.XCoord, a.YCoord)).ToList().GetRandom();
        }
        else if (card.DungeonEventType == DungeonEventType.SpawnOnCorner)
        {
            var tiles = roomArea.GetCornerTiles();
            tile = tiles.GetRandom();
        }
        else if (card.DungeonEventType == DungeonEventType.SpawnOnWideOpen)
        {
            var tiles = roomArea.GetWideOpenTiles();
            tile = tiles.GetRandom();

            if (tile == null)
            {
                // Fall back to corner tiles
                tiles = roomArea.GetCornerTiles();
                tile = tiles.GetRandom();
            }
        }

        return tile;
    }

    public void SpawnEntity(TileEntity entity, GridTile tile)
    {
        Grid.PutObject(tile, entity, true);
    }

    public void SpawnPassableEntity(PassableTileEntity entity, GridTile tile)
    {
        Grid.PutPassableEntity(tile.XCoord, tile.YCoord, entity, true);
    }

    [UsedImplicitly]
    private void Awake()
    {
        Instance = this;
    }

    [UsedImplicitly]
    private void Start()
    {
        Invoke("StartDungeon", 0.5f);
    }

    [UsedImplicitly]
    private void StartDungeon()
    {
        Game.Player.DungeonStarted(this);
    }

    public List<GridTile> GetTilesNearPlayer(TileRangeType rangeType, int range)
    {
        switch (rangeType)
        {
            case TileRangeType.Radial:
                return Grid.GetRadialTileContents(Game.Player.XCoord, Game.Player.YCoord, range).Select(a => a.Tile).ToList();
            case TileRangeType.Sides:
                return Grid.GetSideTileContents(Game.Player.XCoord, Game.Player.YCoord, range).Select(a => a.Tile).ToList();
            default:
                throw new NotImplementedException();
        }
    }

    public List<TileEntity> GetEntitiesNearPlayer(TileRangeType rangeType, int range, TileEntityType? filter = null)
    {
        return Grid.GetEntities(rangeType, Game.Player.XCoord, Game.Player.YCoord, range, filter);        
    }

    public Inventory<PassableTileItem> GetGroundItems()
    {
        var entities = Grid.GetTile(Game.Player.XCoord, Game.Player.YCoord).GetTileItems();
        return entities;
    }
}

