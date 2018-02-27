using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonManager : SingletonObject<DungeonManager>
{    
    public Tile GenericTileTemplate;
    public TileGrid Grid;
    public Room[,] RoomGrid;

    private List<Enemy> enemies = new List<Enemy>();
    private List<Treasure> treasures = new List<Treasure>();

    public Room[] PossibleRoomTemplates;

    public void RemoveEnemy(Enemy enemy)
    {
        this.enemies.Remove(enemy);
        this.Grid.ClearTile(enemy.XCoord, enemy.YCoord);
    }

    public void AfterPlayerTurn()
    {
        if (this.enemies.Count > 0)
        {
            GameManager.Instance.SetState(GameState.EnemyTurn);
            RoutineChain enemyTurns = new RoutineChain(enemies.Select(a => Routine.Create(a.ProcessCharacterTurn)).ToArray());
            enemyTurns.Then(() =>
            {
                GameManager.Instance.SetState(GameState.AwaitingCommand);
            });

            StartCoroutine(enemyTurns);
        }
        else
        {
            GameManager.Instance.SetState(GameState.AwaitingCommand);
        }
    }

    public void PerformDungeonEvents(RoomArea roomArea)
    {      
        // TODO: empty deck?
        List<IDungeonCard> cards = new List<IDungeonCard>();
        cards = DeckManager.Instance.DrawDungeonCards(2);
        foreach (IDungeonCard card in cards)
        {
            switch (card.DungeonEventType)
            {
                case DungeonEventType.SpawnNear:
                case DungeonEventType.SpawnOnCorner:
                case DungeonEventType.SpawnOnWideOpen:
                    PerformSpawnEvent(roomArea, card);
                    break;
                default:
                    Debug.LogError("No behavior set for DungeonEventType!");
                    break;
            }
            
        }

        roomArea.gameObject.SetActive(false);
        PostGroupEventCleanup();
    }

    /// <summary>
    /// Cleanup that happens after all events in a group happen (ie multiple cards).
    /// </summary>
    private void PostGroupEventCleanup()
    {
        Grid.UnreserveAll();
    }

    private void PerformSpawnEvent(RoomArea roomArea, IDungeonCard card) 
    {
        TileGrid grid = Grid;
        Tile tile = null;
        if (card.DungeonEventType == DungeonEventType.SpawnNear)
        {
            List<Tile> tiles = roomArea.GetAreaTiles();
            tile = tiles.Where(a => grid.CanOccupy(a.XCoord, a.YCoord)).ToList().GetRandom();            
        }
        else if (card.DungeonEventType == DungeonEventType.SpawnOnCorner)
        {
            List<Tile> tiles = roomArea.GetCornerTiles();
            tile = tiles.GetRandom();
        }
        else if (card.DungeonEventType == DungeonEventType.SpawnOnWideOpen)
        {
            List<Tile> tiles = roomArea.GetWideOpenTiles();
            tile = tiles.GetRandom();
        }

        if (tile != null)
        {
            tile.IsReserved = true;
            DoAfterCardDraw(() =>
            {
                card.ExecuteTileSpawnEvent(tile);
                card.DestroyCard();
            });
        }
        else
        {
            // TODO: no tile chosen? What do?
        }
    }

    public void PerformLootCardDrawing(int cardNum)
    {
        var lootCards = DeckManager.Instance.DrawLootCards(cardNum);
        foreach (ILootCard card in lootCards)
        {
            switch (card.LootEventType)
            {
                case LootEventType.GainLoot:
                default:
                    DoAfterCardDraw(() =>
                    {
                        card.ExecuteLootGetEvent();
                        card.DestroyCard();
                    });
                    
                    break;
            }
        }
    }

    public void PerformAbilityCardDrawing(int cardNum)
    {
        var abilityCards = DeckManager.Instance.DrawAbilityCards(cardNum);
        foreach (IAbilityCard card in abilityCards)
        {
            DoAfterCardDraw(() =>
            {
                Player.Instance.EquipAbility(card);
                card.DestroyCard();
            });
        }
    }

    public void PerformCharacterCardDrawing(int cardNum)
    {
        var charCards = DeckManager.Instance.DrawCharacterCards(cardNum);
        foreach (ICharacterCard card in charCards)
        {
            switch (card.CharacterCardType)
            {
                case CharacterCardType.AttributeGain:
                default:
                    DoAfterCardDraw(() => 
                    {
                        card.ApplyEffect();
                        card.DestroyCard();
                    });

                    break;
            }
        }
    }

    public void SpawnEnemy(Enemy enemy, Tile tile)
    {        
        this.Grid.PutObject(tile, enemy, true);
        this.enemies.Add(enemy);
    }

    public void SpawnTreasure(Treasure treasure, Tile tile)
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
        GameManager.Instance.SetState(GameState.AwaitingCommand);
        //PerformAbilityCardDrawing(2);
    }

    private void DoAfterCardDraw(Action action)
    {
        GameManager.Instance.EnqueueTriggeredEventAction(TriggeredEvent.CardDrawDone, action);
    }

    public List<TileEntity> GetEntitiesNearPlayer(TileRangeType rangeType, int range, TileEntityType? filter = null)
    {
        switch (rangeType) {
            case TileRangeType.Radial:
                return Grid.GetRadialEntities(Player.Instance.XCoord, Player.Instance.YCoord, range, filter);
            default:
                throw new NotImplementedException();
        }
    }

    // Update is called once per frame
    void Update () {
	}
}

