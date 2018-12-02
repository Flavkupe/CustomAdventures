using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using Assets.Scripts.UI.State;

[RequireComponent(typeof(DungeonGenerator))]
public class Dungeon : SingletonObject<Dungeon>
{
    public DungeonPrefabTemplates Templates;

    public event EventHandler<List<Enemy>> EnemyListChanged;

    public event EventHandler<TileEntity> TileEntityClicked;

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

    private GameContext _context = new GameContext();
    private DungeonStateProvider _stateProvider;

    private readonly List<Enemy> _enemies = new List<Enemy>();
    public List<Enemy> Enemies => _enemies;
    private AnimationStateController AnimationStateController { get; } = new AnimationStateController();
    private DungeonStateController DungeonStateController { get; } = new DungeonStateController();

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

    public void PerformGridSelection(List<TileEntity> entities, EntitySelectionOptions options, ActionOnEntities doOnSelected)
    {
        DungeonStateController.SwitchToTileSelection(GetGameContext(), options, doOnSelected);
    }

    public void EntityClicked(TileEntity tileEntity)
    {
        TileEntityClicked?.Invoke(this, tileEntity);
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

    public void EnqueueAnimation(Routine animation)
    {
        AnimationStateController.AddAnimationRoutine(animation, GetGameContext());
    }

    public GameContext GetGameContext()
    {
        return _context;
    }

    /// <summary>
    /// The entry point for all events that should affect states.
    /// Events here are broadcast to all state controllers.
    /// </summary>
    public void BroadcastEvent<TEventType>(TEventType eventType) where TEventType : struct
    {
        _stateProvider.HandleNewEvent(eventType);
    }

    public void RegisterToBroadcastEvents<TEventType>(EventHandler<TEventType> callback) where TEventType : struct
    {
        _stateProvider.RegisterToBroadcastEvents(callback);
    }

    public void UnregisterFromBroadcastEvents<TEventType>(EventHandler<TEventType> callback) where TEventType : struct
    {
        _stateProvider.UnregisterFromBroadcastEvents(callback);
    }

    public IEnumerable<IStateController> GetStateControllers()
    {
        return _stateProvider.Controllers;
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

        _cardController.CardDrawStart += OnCardDrawStart;
        _cardController.CardDrawEnd += OnCardDrawEnd;
        _cardController.PlayerInputRequested += OnPlayerInputRequested;
        _cardController.PlayerInputAcquired += OnPlayerInputAcquired;
    }

    [UsedImplicitly]
    private void Start()
    {
        _stateProvider = new DungeonStateProvider(this,
            DungeonStateController,
            AnimationStateController,
            _player.StateController,
            Game.UI.UIStateController);

        _context.Dungeon = this;
        _context.Player = _player;
        _context.UI = Game.UI;
        _context.CanPerformAction = _stateProvider.CanPerformAction;

        DungeonStateController.Start();
        AnimationStateController.Start();
    }

    private void OnPlayerInputRequested(object sender, EventArgs e)
    {
        BroadcastEvent(PlayerEventType.MouseInputRequested);
    }

    private void OnPlayerInputAcquired(object sender, EventArgs e)
    {
        BroadcastEvent(PlayerEventType.MouseInputAcquired);
    }

    private void OnCardDrawEnd(object sender, EventArgs e)
    {
        AnimationStateController.HandleNewEvent(AnimationEventType.AnimationEnd, GetGameContext());
    }

    private void OnCardDrawStart(object sender, EventArgs e)
    {
        AnimationStateController.HandleNewEvent(AnimationEventType.AnimationStart, GetGameContext());
    }

    [UsedImplicitly]
    private void Update()
    {
        DungeonStateController.Update(GetGameContext());
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
        Debug.Log("OnLootEventRequested!");
        StartCoroutine(_cardController.PerformLootEvents(e));
    }

    private void OnPlayerEnteredRoom(object sender, RoomArea e)
    {
        StartCoroutine(_cardController.PerformDungeonEvents(e));
    }

    private void HandlePlayerBroadcastEvents(object source, PlayerEventType eventType)
    {
    }

    [UsedImplicitly]
    private void StartDungeon()
    {
        _player.AbilityCardNeeded += OnPlayerAbilityCardNeeded;
        _player.LevelupEvent += OnPlayerLevelupEvent;
        _player.DungeonStarted(this);

        RegisterToBroadcastEvents<PlayerEventType>(HandlePlayerBroadcastEvents);

        // If enemies are present at start, invoke
        if (_enemies.Count > 0)
        {
            EnemyListPopulated?.Invoke(this, null);
        }
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

    public void ShuffleNewCardsIntoDeck<TCardType>(Deck<TCardType> deck, IList<TCardType> cards) where TCardType : class, ICard
    {
        StartCoroutine(_cardController.ShuffleNewCardsIntoDeck(deck, cards, ShuffleMode.SmallFromMouse));
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
