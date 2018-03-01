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
            StateManager.Instance.SetState(GameState.EnemyTurn);
            RoutineChain enemyTurns = new RoutineChain(enemies.Select(a => Routine.Create(a.ProcessCharacterTurn)).ToArray());
            enemyTurns.Then(() =>
            {
                StateManager.Instance.SetState(GameState.AwaitingCommand);
            });

            StartCoroutine(enemyTurns);
        }
        else
        {
            StateManager.Instance.SetState(GameState.AwaitingCommand);
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

    public Routine PerformDungeonEvents(RoomArea roomArea)
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

        return DoCardDraw(cards.Cast<ICard>().ToList(), Game.Decks.DungeonDeckHolder);
    }

    public Routine PerformLootCardDrawing(int cardNum)
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

        return DoCardDraw(lootCards.Cast<ICard>().ToList(), Game.Decks.LootDeckHolder);
    }

    public Routine PerformAbilityCardDrawing(int cardNum)
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

        return DoCardDraw(abilityCards.Cast<ICard>().ToList(), Game.Decks.AbilityDeckHolder);
    }

    public Routine PerformCharacterCardDrawing(int cardNum)
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

        return DoCardDraw(charCards.Cast<ICard>().ToList(), Game.Decks.CharDeckHolder);
    }

    private Routine DoCardDraw(List<ICard> cards, GameObject deckHolder)
    {
        StateManager.Instance.IsPaused = true;

        Routine drawRoutine = Routine.Create(Game.Decks.AnimateCardDraws, cards, deckHolder, 10.0f);
        drawRoutine.Then(() =>
        {
            StateManager.Instance.IsPaused = false;
        });

        StateManager.Instance.EnqueueIfNotState(GameState.CharacterMoving, () => drawRoutine);
        return drawRoutine;
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
        StateManager.Instance.SetState(GameState.AwaitingCommand);
        Game.States.EnqueueRoutine(Routine.Create(PerformCharacterCardDrawing, 2));
    }

    private void DoAfterCardDraw(Action action)
    {
        StateManager.Instance.EnqueueTriggeredEventAction(TriggeredEvent.CardDrawDone, action);
    }

    public List<Tile> GetTilesNearPlayer(TileRangeType rangeType, int range)
    {
        switch (rangeType)
        {
            case TileRangeType.Radial:
                return Grid.GetRadialTileContents(Player.Instance.XCoord, Player.Instance.YCoord, range).Select(a => a.Tile).ToList();
            default:
                throw new NotImplementedException();
        }
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

