using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

public class DungeonManager : SingletonObject<DungeonManager>
{
    public DungeonCardData[] PossibleBossCards;

    public GridTile GenericTileTemplate;
    public TileGrid Grid;
    public Room[,] RoomGrid;

    private List<Enemy> _enemies = new List<Enemy>();

    public void RemoveEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
        Grid.ClearTileEntity(enemy.XCoord, enemy.YCoord);
    }

    public bool IsCombat { get { return _enemies.Count > 0; } }

    public void AfterPlayerTurn()
    {
        if (IsCombat && !Game.Player.PlayerCanMove)
        {
            Game.States.SetState(GameState.EnemyTurn);            
            RoutineChain enemyTurns = new RoutineChain(_enemies.Select(a => Routine.Create(a.ProcessCharacterTurn)).ToArray());
            enemyTurns.Then(() =>
            {
                Game.Player.InitializeCombatTurn();
                Game.States.SetState(GameState.AwaitingCommand);
            });

            Game.States.EnqueueCoroutine(enemyTurns);
        }
        else
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

    public void PerformSpawnEvent(RoomArea roomArea, IDungeonCard card) 
    {
        int numTimes = card.GetNumberOfExecutions();   

        bool wasCombatActive = IsCombat;
        TileGrid grid = Grid;

        for (int i = 0; i < numTimes; i++)
        {
            GridTile tile = null;
            if (card.DungeonEventType == DungeonEventType.SpawnNear)
            {
                List<GridTile> tiles = roomArea.GetAreaTiles();
                tile = tiles.Where(a => grid.CanOccupy(a.XCoord, a.YCoord)).ToList().GetRandom();
            }
            else if (card.DungeonEventType == DungeonEventType.SpawnOnCorner)
            {
                List<GridTile> tiles = roomArea.GetCornerTiles();
                tile = tiles.GetRandom();
            }
            else if (card.DungeonEventType == DungeonEventType.SpawnOnWideOpen)
            {
                List<GridTile> tiles = roomArea.GetWideOpenTiles();
                tile = tiles.GetRandom();

                if (tile == null)
                {
                    // Fall back to corner tiles
                    tiles = roomArea.GetCornerTiles();
                    tile = tiles.GetRandom();
                }
            }

            if (tile != null)
            {                
                if (card.RequiresFullTile)
                {
                    // TODO: I don't think the IsReserved system is needed....
                    tile.IsReserved = true;
                }

                card.ExecuteTileSpawnEvent(tile);
            }
            else
            {
                // TODO: no tile chosen? What do?
            }
        }

        if (!wasCombatActive && IsCombat)
        {
            InitializeCombat();
        }

        card.DestroyCard();
    }

    private void InitializeCombat()
    {
        Game.Player.InitializeCombatTurn();
    }

    public void SpawnEnemy(Enemy enemy, GridTile tile)
    {
        Grid.PutObject(tile, enemy, true);
        _enemies.Add(enemy);
    }

    public void SpawnTreasure(Treasure treasure, GridTile tile)
    {
        Grid.PutObject(tile, treasure, true);
    }

    public void SpawnTrap(TileTrap trap, GridTile tile)
    {
        Grid.PutPassableEntity(tile.XCoord, tile.YCoord, trap, true);
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
        Game.States.SetState(GameState.AwaitingCommand);
        //Game.States.EnqueueRoutine(Routine.CreateAction(Game.CardDraw.PerformCharacterCardDrawing, 2));
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

