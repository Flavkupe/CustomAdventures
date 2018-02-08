using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonManager : SingletonObject<DungeonManager>
{    
    public Tile GenericTileTemplate;
    
    public int RoomDims = 9;

    public Enemy EnemyTemplate;

    public TileGrid Grid;

    public Room[,] RoomGrid;

    public int NumRooms = 4;

    public int Dims { get { return this.RoomDims * NumRooms; } }

    public Room[] PossibleRoomTemplates;

    private List<Enemy> enemies = new List<Enemy>();
    private List<Treasure> treasures = new List<Treasure>();

    private Queue<Action> postAnimationActionQueue = new Queue<Action>();

    private void CreateRooms()
    {
        this.Grid.Init(Dims, Dims);
        List<Tile> openConnectors = new List<Tile>();
        List<Tile> toClearLater = new List<Tile>();
            
        RoomGrid = new Room[NumRooms, NumRooms];

        // First room needs both a down and either a right or an up
        Room firstRoom = Instantiate(this.PossibleRoomTemplates.FirstOrDefault(a => a.HasConnectorToDirection(Direction.Down) &&
          (a.HasConnectorToDirection(Direction.Right) || a.HasConnectorToDirection(Direction.Up))));
        firstRoom.InitRoomTiles();
        firstRoom.XCoord = 0;
        firstRoom.YCoord = 0;
        firstRoom.transform.position = new Vector3(0, 0);             
        RoomGrid[0, 0] = firstRoom;

        // For first tile, ignore the down connector
        Tile[] tiles = firstRoom.GetTiles();
        Tile startingTile = tiles.First(a => a.ConnectsTo == Direction.Down);
        openConnectors.AddRange(tiles.Where(a => a.IsConnectorTile() && a.ConnectsTo != Direction.Down));
        int numRoomsCreated = 1;
        while(true)
        {
            // Clear out the bad connectors
            foreach (Tile connectorToCheck in openConnectors.ToList())
            {
                Room room = connectorToCheck.GetRoom();
                if (RoomGrid.IsAdjacentOffBoundsOrFull(room.XCoord, room.YCoord, connectorToCheck.ConnectsTo.Value))
                {                    
                    toClearLater.Add(connectorToCheck);
                    openConnectors.Remove(connectorToCheck);
                }
                else if (!this.PossibleRoomTemplates.Any(a => a.HasExactMatchingConnector(connectorToCheck)))
                {
                    // No possible connectors for this
                    toClearLater.Add(connectorToCheck);
                    openConnectors.Remove(connectorToCheck);                   
                }
            }

            if (numRoomsCreated == this.NumRooms)
            {
                break;
            }

            if (openConnectors.Count == 0)
            {
                // If no possible connectors, throw it away and start over
                Utils.DoForXY(NumRooms, NumRooms, (x, y) =>
                {
                    if (x != 0 || y != 0)
                    {
                        Room toDelete = this.RoomGrid[x, y];
                        if (toDelete != null)
                        {
                            Destroy(toDelete.gameObject);
                        }
                    }
                });
            }
            else
            {
                Tile connector = openConnectors.GetRandom();
                openConnectors.Remove(connector);
                Room currRoom = connector.GetRoom();
                IEnumerable<Room> possibleRooms = this.PossibleRoomTemplates.Where(a => a.HasExactMatchingConnector(connector));
                Room newRoom = Instantiate(possibleRooms.ToList().GetRandom());
                newRoom.InitRoomTiles();
                newRoom.XCoord = currRoom.XCoord;
                newRoom.YCoord = currRoom.YCoord;
                newRoom.ShiftInDirection(connector.ConnectsTo.Value);
                RoomGrid[newRoom.XCoord, newRoom.YCoord] = newRoom;
                newRoom.transform.position = new Vector3(newRoom.XCoord * RoomDims, newRoom.YCoord * RoomDims);
                Tile[] newTiles = newRoom.GetTiles();
                Direction opposite = Utils.GetOppositeDirection(connector.ConnectsTo.Value);
                openConnectors.AddRange(newTiles.Where(a => a.IsConnectorTile() && !(a.ConnectsTo.Value == opposite)));
                numRoomsCreated++;
            }
        }

        // Put all tiles in grid.
        Utils.DoForXY(NumRooms, NumRooms, (x, y) =>
        {
            Room room = this.RoomGrid[x, y];
            if (room != null)
            {
                int roomOffsetX = room.XCoord * RoomDims;
                int roomOffsetY = room.YCoord * RoomDims; 
                foreach (Tile tile in room.GetTiles())
                {
                    tile.XCoord = roomOffsetX + (int)tile.transform.localPosition.x;
                    tile.YCoord = roomOffsetY + (int)tile.transform.localPosition.y;
                    this.Grid.PutTile(tile);
                }
            }
        });

        // Remove all left-over connectors
        foreach (Tile connector in openConnectors)
        {
            DestroyConnectorNeighborsRecursive(connector);
        }

        foreach (Tile connector in toClearLater)
        {
            DestroyConnectorNeighborsRecursive(connector);
        }

        this.Grid.PutObject(startingTile.XCoord, startingTile.YCoord, Player.Instance, true);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        this.enemies.Remove(enemy);
        this.Grid.ClearTile(enemy.XCoord, enemy.YCoord);
    }

    private HashSet<Tile> visitedNeighbors = new HashSet<Tile>();
    private void DestroyConnectorNeighborsRecursive(Tile tile)
    {
        if (tile == null)
        {
            return;
        }

        if (visitedNeighbors.Contains(tile))
        {
            return;
        }
       
        visitedNeighbors.Add(tile);

        int x = tile.XCoord;
        int y = tile.YCoord;
        Destroy(tile.gameObject);
        foreach (Tile neighbor in this.Grid.GetNeighbors(x, y))
        {
            if (neighbor != null && (neighbor.IsConnectorTile() || neighbor.IsConnectorNeighbor))
            {
                DestroyConnectorNeighborsRecursive(neighbor);
            }            
        }        
    }  

    public void AfterPlayerMove()
    {
        foreach (Enemy enemy in this.enemies)
        {
            enemy.MoveAfterPlayer();
        }
    }

    public void PerformDungeonEvents(RoomArea roomArea)
    {
        List<Tile> tiles = roomArea.GetAreaTiles();

        // TODO: empty deck?
        List<IDungeonCard> cards = new List<IDungeonCard>();
        cards = DeckManager.Instance.DrawDungeonCards(2);
        TileGrid grid = Grid;
        foreach (IDungeonCard card in cards)
        {
            if (card.DungeonEventType == DungeonEventType.SpawnNear)
            {
                Tile tile = tiles.Where(a => grid.CanOccupy(a.XCoord, a.YCoord)).ToList().GetRandom();
                if (tile != null)
                {                    
                    postAnimationActionQueue.Enqueue(() =>
                    {
                        card.ExecuteTileSpawnEvent(tile);
                        card.DestroyCard();
                    });
                }
            }
        }

        roomArea.gameObject.SetActive(false);                
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
        DeckManager.Instance.OnDrawAnimationDone += HandleOnDrawAnimationDone;
        this.CreateRooms();
    }

    private void HandleOnDrawAnimationDone(object sender, EventArgs e)
    {
        Action action = null;
        while (postAnimationActionQueue.Count > 0)            
        {
            action = postAnimationActionQueue.Dequeue();
            action.Invoke();
        }

        GameManager.Instance.IsPaused = false;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
