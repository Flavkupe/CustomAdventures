using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

[RequireComponent(typeof(DungeonGenerator))]
public class Dungeon : SingletonObject<Dungeon>
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

    public GridTile GenericTileTemplate;
    public TileGrid Grid;

    // TODO: remove singleton
    public DeckManager Decks => Game.Decks;

    private List<Room> _rooms;

    private CardEventController _cardController;

    private Player _player;

    private AnimationStateController _animationStateController = new AnimationStateController();
    private readonly DungeonStateController _dungeonStateController = new DungeonStateController();

    private GameContext _context = new GameContext();

    private readonly List<Enemy> _enemies = new List<Enemy>();
    public List<Enemy> Enemies => _enemies;


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

    private List<TileEntity> _validSelectionTargets;
    private readonly List<TileEntity> _selectedTargets = new List<TileEntity>();
    public List<TileEntity> SelectedTargets => _selectedTargets;

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

    /// <summary>
    /// Gets a random unvisited room area. If all rooms have been visited, returns 'defaultArea'.
    /// </summary>
    /// <param name="defaultArea">RoomArea to get if all areas have been explored.</param>
    /// <returns>Random unvisited RoomArea, or defaultArea if none exist.</returns>
    public RoomArea GetUnexploredRoomArea(RoomArea defaultArea)
    {
        var areas = new List<RoomArea>();
        foreach (var room in _rooms)
        {
            if (room != null)
            {
                var roomAreas = room.GetComponentsInChildren<RoomArea>();
                areas.AddRange(roomAreas.Where(a => !a.RoomVisited));
            }
        }

        return areas.Count == 0 ? defaultArea : areas.GetRandom();
    }

    /// <summary>
    /// Spawns an entity that can produce loot draw results, and observes those results.
    /// Use this instead of SpawnEntity to watch for loot draws.
    /// </summary>
    public void SpawnLootableEntity<TEntityType>(TEntityType entity, GridTile tile) where TEntityType : TileEntity, IProducesLootEvent
    {
        SpawnEntity(entity, tile);
        entity.LootEventRequested += OnLootEventRequested;
    }

    public void SpawnEntity(TileEntity entity, GridTile tile)
    {
        Grid.PutObject(tile, entity, true);
    }

    public void SpawnPassableEntity(PassableTileEntity entity, GridTile tile)
    {
        Grid.PutPassableEntity(tile.XCoord, tile.YCoord, entity, true);
    }

    public GameContext GetGameContext()
    {
        return _context;
    }

    [UsedImplicitly]
    private void Awake()
    {
        Instance = this;
        var generator = GetComponent<DungeonGenerator>();
        generator.DungeonGenerated += OnDungeonGenerated;

        _cardController = GetComponentInChildren<CardEventController>();
        Debug.Assert(_cardController != null, "Dungeon needs a CardEventController child object!");

        _player = FindObjectOfType<Player>();
        Debug.Assert(_player != null, "Could not find Player object!");
    }

    [UsedImplicitly]
    private void Start()
    {
        _context.Dungeon = this;
        _context.Player = _player;
        _context.UI = Game.UI;

        _dungeonStateController.Start();
    }

    [UsedImplicitly]
    private void Update()
    {
        _dungeonStateController.Update(GetGameContext());
    }

    /// <summary>
    /// Called when dungeon is done generating
    /// </summary>
    private void OnDungeonGenerated(object sender, List<Room> e)
    {
        // Get generated rooms and create listeners for room events
        _rooms = e;
        foreach (var room in _rooms)
        {
            room.PlayerEnteredRoom += OnPlayerEnteredRoom;
        }

        StartDungeon();
    }

    private void OnPlayerAbilityCardNeeded(object sender, Player e)
    {
        StartCoroutine(_cardController.PerformAbilityCardEvents(e));
    }

    private void OnPlayerLevelupEvent(object sender, Player e)
    {
        StartCoroutine(_cardController.PerformCharacterCardEvents(e));
    }

    private void OnLootEventRequested(object sender, LootEventProperties e)
    {
        StartCoroutine(_cardController.PerformLootEvents(e));
    }

    private void OnPlayerEnteredRoom(object sender, RoomArea e)
    {
        StartCoroutine(_cardController.PerformDungeonEvents(e));
    }

    private void OnPlayerActionTaken(object sender, EventArgs e)
    {
        _dungeonStateController.SendEvent(DungeonEventType.AfterPlayerAction, GetGameContext());
    }

    [UsedImplicitly]
    private void StartDungeon()
    {
        _player.AbilityCardNeeded += OnPlayerAbilityCardNeeded;
        _player.LevelupEvent += OnPlayerLevelupEvent;
        _player.AfterPlayerAction += OnPlayerActionTaken;
        _player.DungeonStarted(this);
    }

    public List<GridTile> GetTilesNearPlayer(TileRangeType rangeType, int range)
    {
        switch (rangeType)
        {
            case TileRangeType.Radial:
                return Grid.GetRadialTileContents(_player.XCoord, _player.YCoord, range).Select(a => a.Tile).ToList();
            case TileRangeType.Sides:
                return Grid.GetSideTileContents(_player.XCoord, _player.YCoord, range).Select(a => a.Tile).ToList();
            default:
                throw new NotImplementedException();
        }
    }

    public List<TileEntity> GetEntitiesNearPlayer(TileRangeType rangeType, int range, TileEntityType? filter = null)
    {
        return Grid.GetEntities(rangeType, _player.XCoord, _player.YCoord, range, filter);        
    }

    public Inventory<PassableTileItem> GetGroundItems()
    {
        var entities = Grid.GetTile(_player.XCoord, _player.YCoord).GetTileItems();
        return entities;
    }

    private int _pauseCounters;
    public void PauseActions()
    {
        _pauseCounters++;
    }

    public void UnpauseActions()
    {
        _pauseCounters--;
    }

    public bool IsGamePaused => _pauseCounters > 0;
}
