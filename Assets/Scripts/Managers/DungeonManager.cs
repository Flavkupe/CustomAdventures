using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonManager : SingletonObject<DungeonManager>
{
    public GridTile GenericTileTemplate;
    public TileGrid Grid;
    public Room[,] RoomGrid;

    private List<Enemy> enemies = new List<Enemy>();
    private List<Treasure> treasures = new List<Treasure>();

    public void RemoveEnemy(Enemy enemy)
    {
        this.enemies.Remove(enemy);
        this.Grid.ClearTile(enemy.XCoord, enemy.YCoord);
    }

    public void AfterPlayerTurn()
    {
        if (this.enemies.Count > 0)
        {
            Game.States.SetState(GameState.EnemyTurn);
            RoutineChain enemyTurns = new RoutineChain(enemies.Select(a => Routine.Create(a.ProcessCharacterTurn)).ToArray());
            enemyTurns.Then(() =>
            {
                Game.States.SetState(GameState.AwaitingCommand);
            });

            Game.States.EnqueueCoroutine(enemyTurns);
        }
        else
        {
            Game.States.SetState(GameState.AwaitingCommand);
        }
    }

    private List<TileEntity> validSelectionTargets = null;
    private List<TileEntity> selectedTargets = new List<TileEntity>();
    public List<TileEntity> SelectedTargets { get { return selectedTargets; } }
    public IEnumerator AwaitTargetSelection(Action cancellationCallback, List<TileEntity> entities, int numToSelect)
    {        
        this.selectedTargets.Clear();
        this.validSelectionTargets = entities;
        //numToSelect = Math.Min(this.validSelectionTargets.Count, numToSelect);
        //if (numToSelect == 0)
        //{
        //    // Nothing available to select!
        //    cancellationCallback();
        //    yield break;
        //}

        Game.States.SetState(GameState.AwaitingSelection);

        while (this.selectedTargets.Count < numToSelect)
        {
            if (Input.GetMouseButtonUp(1))
            {
                cancellationCallback();
                yield break;
            }

            yield return null;
        }

        foreach (TileEntity target in this.selectedTargets)
        {
            // Reset selected state after all found
            target.Selected = false;
        }
    }

    public void AfterToggledSelection(TileEntity tileEntity)
    {
        if (Game.States.State != GameState.AwaitingSelection || !this.validSelectionTargets.Contains(tileEntity))
        {
            tileEntity.Selected = false;
            return;
        }

        if (!tileEntity.Selected && this.selectedTargets.Contains(tileEntity))
        {
            this.selectedTargets.Remove(tileEntity);
        }
        else if (tileEntity.Selected && !this.selectedTargets.Contains(tileEntity))
        {
            this.selectedTargets.Add(tileEntity);
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
        TileGrid grid = Grid;
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
        }

        if (tile != null)
        {
            tile.IsReserved = true;
            card.ExecuteTileSpawnEvent(tile);
            card.DestroyCard();
        }
        else
        {
            // TODO: no tile chosen? What do?
        }
    }

    public void SpawnEnemy(Enemy enemy, GridTile tile)
    {        
        this.Grid.PutObject(tile, enemy, true);
        this.enemies.Add(enemy);
    }

    public void SpawnTreasure(Treasure treasure, GridTile tile)
    {
        this.Grid.PutObject(tile, treasure, true);
        this.treasures.Add(treasure);
    }

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        Invoke("StartDungeon", 0.5f);
    }

    private void StartDungeon()
    {
        Game.States.SetState(GameState.AwaitingCommand);
        //Game.States.EnqueueRoutine(Routine.Create(Game.CardDraw.PerformCharacterCardDrawing, 2));
    }

    public List<GridTile> GetTilesNearPlayer(TileRangeType rangeType, int range)
    {
        switch (rangeType)
        {
            case TileRangeType.Radial:
                return Grid.GetRadialTileContents(Game.Player.XCoord, Game.Player.YCoord, range).Select(a => a.Tile).ToList();
            default:
                throw new NotImplementedException();
        }
    }

    public List<TileEntity> GetEntitiesNearPlayer(TileRangeType rangeType, int range, TileEntityType? filter = null)
    {
        switch (rangeType) {
            case TileRangeType.Radial:
                return Grid.GetRadialEntities(Game.Player.XCoord, Game.Player.YCoord, range, filter);
            default:
                throw new NotImplementedException();
        }
    }

    // Update is called once per frame
    void Update () {
	}
}

