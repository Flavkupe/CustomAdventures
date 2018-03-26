using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class DungeonGenerator: SingletonObject<DungeonGenerator>
{
    public int NumRooms = 4;
    public int RoomDims = 16;
    public int Dims { get { return this.RoomDims * NumRooms; } }
    public GridTile GenericTileTemplate;
    public Room[] PossibleRoomTemplates;

    private TileGrid grid = null;
    private Room[,] roomGrid = null;

    private List<GridTile> _openConnectors = new List<GridTile>();
    private List<GridTile> _toClearLater = new List<GridTile>();
    private int _numRoomsCreated = 0;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        this.CreateRooms();
    }

    private void CreateRooms()
    {
        // At least 2 rooms (start and boss rooms)
        this.NumRooms = Mathf.Max(this.NumRooms, 2);

        this.roomGrid = Game.Dungeon.RoomGrid;
        this.grid = Game.Dungeon.Grid;

        this.grid.Init(Dims, Dims);

        roomGrid = new Room[NumRooms, NumRooms];

        // First room needs both a down and either a right or an up
        Room firstRoom = Instantiate(this.PossibleRoomTemplates.Where(a => !a.BossRoom).FirstOrDefault(a => a.HasConnectorToDirection(Direction.Down) &&
          (a.HasConnectorToDirection(Direction.Right) || a.HasConnectorToDirection(Direction.Up))));
        firstRoom.InitRoomTiles();
        firstRoom.XCoord = 0;
        firstRoom.YCoord = 0;
        firstRoom.transform.position = new Vector3(0, 0);
        roomGrid[0, 0] = firstRoom;

        // For first tile, ignore the down connector
        GridTile[] tiles = firstRoom.GetTiles();
        GridTile startingTile = tiles.First(a => a.ConnectsTo == Direction.Down);
        _openConnectors.AddRange(tiles.Where(a => a.IsConnectorTile() && a.ConnectsTo != Direction.Down));
        _numRoomsCreated = 1;

        bool bossTime = false;
        int attempts = 0;
        while (attempts < 5000)
        {
            if (++attempts >= 5000)
            {
                // TODO: better failsafe
                Debug.Assert(false, "Uhoh, could not create map!");
                Application.Quit();
            }

            if (_numRoomsCreated == this.NumRooms - 1)
            {
                bossTime = true;
            }

            if (_numRoomsCreated >= this.NumRooms)
            {
                break;
            }

            // Clear out the bad connectors
            foreach (GridTile connectorToCheck in _openConnectors.ToList())
            {
                Room room = connectorToCheck.GetRoom();
                if (roomGrid.IsAdjacentOffBoundsOrFull(room.XCoord, room.YCoord, connectorToCheck.ConnectsTo.Value))
                {
                    _toClearLater.Add(connectorToCheck);
                    _openConnectors.Remove(connectorToCheck);
                }
                else if (!this.PossibleRoomTemplates.Where(a => a.BossRoom == bossTime).Any(a => a.HasExactMatchingConnector(connectorToCheck)))
                {
                    // No possible connectors for this
                    _toClearLater.Add(connectorToCheck);
                    _openConnectors.Remove(connectorToCheck);
                }
            }

            if (_openConnectors.Count == 0)
            {
                // If no possible connectors, throw it away and start over
                Utils.DoForXY(NumRooms, NumRooms, (x, y) =>
                {
                    if (x != 0 || y != 0)
                    {
                        Room toDelete = roomGrid[x, y];
                        if (toDelete != null)
                        {
                            Destroy(toDelete.gameObject);
                        }
                    }
                });
            }
            else
            {
                MakeNewRoom(bossTime);
            }
        }

        // Put all tiles in this.grid.
        Utils.DoForXY(NumRooms, NumRooms, (x, y) =>
        {
            Room room = roomGrid[x, y];
            if (room != null)
            {
                int roomOffsetX = room.XCoord * RoomDims;
                int roomOffsetY = room.YCoord * RoomDims;
                foreach (GridTile tile in room.GetTiles())
                {
                    tile.XCoord = roomOffsetX + (int)tile.transform.localPosition.x;
                    tile.YCoord = roomOffsetY + (int)tile.transform.localPosition.y;
                    tile.name = string.Format("Tile_{0}_{1}", tile.XCoord, tile.YCoord);
                    this.grid.PutTile(tile);
                }

                if (room.Pathing != null)
                {
                    room.Pathing.GetComponent<TilemapRenderer>().enabled = false;
                }
            }
        });

        // Remove all left-over connectors
        foreach (GridTile connector in _openConnectors)
        {
            DestroyConnectorNeighborsRecursive(connector);
        }

        foreach (GridTile connector in _toClearLater)
        {
            DestroyConnectorNeighborsRecursive(connector);
        }

        this.grid.PutObject(startingTile.XCoord, startingTile.YCoord, Game.Player, true);
    }

    private void MakeNewRoom(bool bossRoom)
    {
        GridTile connector = null;
        if (bossRoom)
        {
            connector = _openConnectors.GetMax(a => Vector3.Distance(a.transform.position, Game.Player.transform.position));
        }
        else
        {
            connector = _openConnectors.GetRandom();
        }

        _openConnectors.Remove(connector);
        Room currRoom = connector.GetRoom();
        List<Room> possibleRooms = this.PossibleRoomTemplates.Where(a => a.BossRoom == bossRoom && a.HasExactMatchingConnector(connector)).ToList();
        if (possibleRooms.Count == 0)
        {
            // No rooms for this connector; try again
            return;
        }

        Room newRoom = Instantiate(possibleRooms.ToList().GetRandom());
        newRoom.InitRoomTiles();        
        newRoom.XCoord = currRoom.XCoord;
        newRoom.YCoord = currRoom.YCoord;
        newRoom.ShiftInDirection(connector.ConnectsTo.Value);
        roomGrid[newRoom.XCoord, newRoom.YCoord] = newRoom;
        newRoom.transform.position = new Vector3(newRoom.XCoord * RoomDims, newRoom.YCoord * RoomDims);
        GridTile[] newTiles = newRoom.GetTiles();
        Direction opposite = Utils.GetOppositeDirection(connector.ConnectsTo.Value);
        var newConnectors = newTiles.Where(a => a.IsConnectorTile());
        var matching = newRoom.GetExactMatchingConnector(connector);
        _openConnectors.AddRange(newConnectors.Where(a => !(a.ConnectsTo.Value == opposite)));
        _toClearLater.AddRange(newConnectors.Where(a => a != matching && a.ConnectsTo.Value == opposite));
        _numRoomsCreated++;
    }

    private HashSet<GridTile> visitedNeighbors = new HashSet<GridTile>();
    private void DestroyConnectorNeighborsRecursive(GridTile tile)
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
        tile.RemoveConnector();        
        foreach (GridTile neighbor in this.grid.GetNeighbors(x, y))
        {
            if (neighbor != null && (neighbor.IsConnectorTile() || neighbor.IsConnectorNeighbor))
            {
                DestroyConnectorNeighborsRecursive(neighbor);
            }
        }
    }
}
