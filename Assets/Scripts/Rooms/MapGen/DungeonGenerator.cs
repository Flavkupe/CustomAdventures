using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonGenerator: SingletonObject<DungeonGenerator>
{
    public int NumRooms = 4;
    public int RoomDims = 16;
    public int Dims { get { return this.RoomDims * NumRooms; } }
    public Tile GenericTileTemplate;
    public Room[] PossibleRoomTemplates;

    private TileGrid grid = null;
    private Room[,] roomGrid = null;

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
        this.roomGrid = DungeonManager.Instance.RoomGrid;
        this.grid = DungeonManager.Instance.Grid;

        this.grid.Init(Dims, Dims);
        List<Tile> openConnectors = new List<Tile>();
        List<Tile> toClearLater = new List<Tile>();

        roomGrid = new Room[NumRooms, NumRooms];

        // First room needs both a down and either a right or an up
        Room firstRoom = Instantiate(this.PossibleRoomTemplates.FirstOrDefault(a => a.HasConnectorToDirection(Direction.Down) &&
          (a.HasConnectorToDirection(Direction.Right) || a.HasConnectorToDirection(Direction.Up))));
        firstRoom.InitRoomTiles();
        firstRoom.XCoord = 0;
        firstRoom.YCoord = 0;
        firstRoom.transform.position = new Vector3(0, 0);
        roomGrid[0, 0] = firstRoom;

        // For first tile, ignore the down connector
        Tile[] tiles = firstRoom.GetTiles();
        Tile startingTile = tiles.First(a => a.ConnectsTo == Direction.Down);
        openConnectors.AddRange(tiles.Where(a => a.IsConnectorTile() && a.ConnectsTo != Direction.Down));
        int numRoomsCreated = 1;
        while (true)
        {
            // Clear out the bad connectors
            foreach (Tile connectorToCheck in openConnectors.ToList())
            {
                Room room = connectorToCheck.GetRoom();
                if (roomGrid.IsAdjacentOffBoundsOrFull(room.XCoord, room.YCoord, connectorToCheck.ConnectsTo.Value))
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
                Tile connector = openConnectors.GetRandom();
                openConnectors.Remove(connector);
                Room currRoom = connector.GetRoom();
                IEnumerable<Room> possibleRooms = this.PossibleRoomTemplates.Where(a => a.HasExactMatchingConnector(connector));
                Room newRoom = Instantiate(possibleRooms.ToList().GetRandom());
                newRoom.InitRoomTiles();
                newRoom.XCoord = currRoom.XCoord;
                newRoom.YCoord = currRoom.YCoord;
                newRoom.ShiftInDirection(connector.ConnectsTo.Value);
                roomGrid[newRoom.XCoord, newRoom.YCoord] = newRoom;
                newRoom.transform.position = new Vector3(newRoom.XCoord * RoomDims, newRoom.YCoord * RoomDims);
                Tile[] newTiles = newRoom.GetTiles();
                Direction opposite = Utils.GetOppositeDirection(connector.ConnectsTo.Value);
                openConnectors.AddRange(newTiles.Where(a => a.IsConnectorTile() && !(a.ConnectsTo.Value == opposite)));
                numRoomsCreated++;
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
                foreach (Tile tile in room.GetTiles())
                {
                    tile.XCoord = roomOffsetX + (int)tile.transform.localPosition.x;
                    tile.YCoord = roomOffsetY + (int)tile.transform.localPosition.y;
                    this.grid.PutTile(tile);
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

        this.grid.PutObject(startingTile.XCoord, startingTile.YCoord, Player.Instance, true);
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
        foreach (Tile neighbor in this.grid.GetNeighbors(x, y))
        {
            if (neighbor != null && (neighbor.IsConnectorTile() || neighbor.IsConnectorNeighbor))
            {
                DestroyConnectorNeighborsRecursive(neighbor);
            }
        }
    }
}
