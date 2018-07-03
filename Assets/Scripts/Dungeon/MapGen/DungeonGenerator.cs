using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Dungeon))]
[RequireComponent(typeof(TileGrid))]
public class DungeonGenerator: MonoBehaviourEx
{
    public int NumRooms = 4;
    public int RoomDims = 16;
    public int Dims => RoomDims * NumRooms;
    public GridTile GenericTileTemplate;
    public Room[] PossibleRoomTemplates;

    private TileGrid _grid;
    private Room[,] _roomGrid;

    private readonly List<GridTile> _openConnectors = new List<GridTile>();
    private readonly List<GridTile> _toClearLater = new List<GridTile>();
    private int _numRoomsCreated;

    private Dungeon _dungeon;

    [UsedImplicitly]
    private void Start()
    {
        _dungeon = GetComponent<Dungeon>();

        if (PossibleRoomTemplates.Length == 0)
        {
            var allRooms = Resources.LoadAll<Room>("Rooms");
            var rooms = allRooms.Where(a => a.IncludeRoom && a.Dims == this.RoomDims);
            PossibleRoomTemplates = rooms.ToArray();
        }

        CreateRooms();
    }

    private void CreateRooms()
    {
        // At least 2 rooms (start and boss rooms)
        NumRooms = Mathf.Max(NumRooms, 2);

        _grid = _dungeon.Grid;

        _grid.Init(Dims, Dims);

        _roomGrid = new Room[NumRooms, NumRooms];
        _dungeon.RoomGrid = _roomGrid;

        // First room needs a down
        var firstRoomTemplate = PossibleRoomTemplates.Where(a => a.EntranceRoom).FirstOrDefault(a => a.HasConnectorToDirection(Direction.Down));
        var firstRoom = Instantiate(firstRoomTemplate);
        firstRoom.InitRoom(_dungeon, _dungeon.Templates.DungeonParts.GridTile);
        firstRoom.XCoord = 0;
        firstRoom.YCoord = 0;
        firstRoom.transform.position = new Vector3(0, 0);
        _roomGrid[0, 0] = firstRoom;

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

            if (_numRoomsCreated == NumRooms - 1)
            {
                bossTime = true;
            }

            if (_numRoomsCreated >= NumRooms)
            {
                break;
            }

            // Clear out the bad connectors
            foreach (GridTile connectorToCheck in _openConnectors.ToList())
            {
                Room room = connectorToCheck.GetRoom();
                if (connectorToCheck.ConnectsTo.HasValue && _roomGrid.IsAdjacentOffBoundsOrFull(room.XCoord, room.YCoord, connectorToCheck.ConnectsTo.Value))
                {
                    _toClearLater.Add(connectorToCheck);
                    _openConnectors.Remove(connectorToCheck);
                }
                else if (!PossibleRoomTemplates.Where(a => bossTime ? a.BossRoom : a.IsNormalRoom).Any(a => a.HasExactMatchingConnector(connectorToCheck)))
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
                        Room toDelete = _roomGrid[x, y];
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

        // Put all tiles in this.grid and populate props
        Utils.DoForXY(NumRooms, NumRooms, (x, y) =>
        {
            Room room = _roomGrid[x, y];
            if (room != null)
            {
                int roomOffsetX = room.XCoord * RoomDims;
                int roomOffsetY = room.YCoord * RoomDims;
                foreach (GridTile tile in room.GetTiles())
                {
                    tile.XCoord = roomOffsetX + (int)tile.transform.localPosition.x;
                    tile.YCoord = roomOffsetY + (int)tile.transform.localPosition.y;
                    tile.name = $"Tile_{tile.XCoord}_{tile.YCoord}";
                    _grid.PutTile(tile);
                }

                if (room.Pathing != null)
                {
                    room.Pathing.GetComponent<TilemapRenderer>().enabled = false;
                }

                room.PopulateProps();
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

        _grid.PutObject(startingTile.XCoord, startingTile.YCoord, Game.Player, true);
    }

    private void MakeNewRoom(bool bossRoom)
    {
        GridTile connector = null;
        connector = bossRoom ? _openConnectors.GetMax(a => Vector3.Distance(a.transform.position, Game.Player.transform.position)) : _openConnectors.GetRandom();

        _openConnectors.Remove(connector);
        Room currRoom = connector.GetRoom();
        List<Room> possibleRooms = PossibleRoomTemplates.Where(a => bossRoom ? a.BossRoom : a.IsNormalRoom && a.HasExactMatchingConnector(connector)).ToList();
        if (possibleRooms.Count == 0)
        {
            // No rooms for this connector; try again
            return;
        }

        Room newRoom = Instantiate(possibleRooms.ToList().GetRandom());
        newRoom.InitRoom(_dungeon, _dungeon.Templates.DungeonParts.GridTile);
        newRoom.XCoord = currRoom.XCoord;
        newRoom.YCoord = currRoom.YCoord;
        newRoom.ShiftInDirection(connector.ConnectsTo.Value);
        _roomGrid[newRoom.XCoord, newRoom.YCoord] = newRoom;
        newRoom.transform.position = new Vector3(newRoom.XCoord * RoomDims, newRoom.YCoord * RoomDims);
        GridTile[] newTiles = newRoom.GetTiles();
        Direction opposite = Utils.GetOppositeDirection(connector.ConnectsTo.Value);
        var newConnectors = newTiles.Where(a => a.IsConnectorTile()).ToList();
        var matching = newRoom.GetExactMatchingConnector(connector);
        _openConnectors.AddRange(newConnectors.Where(a => a.ConnectsTo != null && a.ConnectsTo.Value != opposite));
        _toClearLater.AddRange(newConnectors.Where(a => a != matching && a.ConnectsTo != null && a.ConnectsTo.Value == opposite));
        _numRoomsCreated++;
    }

    private readonly HashSet<GridTile> _visitedNeighbors = new HashSet<GridTile>();

    private void DestroyConnectorNeighborsRecursive(GridTile tile)
    {
        if (tile == null)
        {
            return;
        }

        if (_visitedNeighbors.Contains(tile))
        {
            return;
        }

        _visitedNeighbors.Add(tile);

        int x = tile.XCoord;
        int y = tile.YCoord;
        tile.RemoveConnector();        
        foreach (var neighbor in _grid.GetNeighbors(x, y))
        {
            if (neighbor != null && (neighbor.IsConnectorTile() || neighbor.IsConnectorNeighbor))
            {
                DestroyConnectorNeighborsRecursive(neighbor);
            }
        }
    }
}
