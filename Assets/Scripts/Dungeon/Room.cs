using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour, IHasCoords
{
    /// <summary>
    /// The NxN dimensions for the rooms.
    /// </summary>
    public int Dims = 9;

    public bool IncludeRoom = true;

    public Tilemap Pathing;
    public Tilemap Entities;

    public bool BossRoom = false;
    public bool EntranceRoom = false;

    public bool IsNormalRoom => !BossRoom && !EntranceRoom;

    public int XCoord { get; set; }
    public int YCoord { get; set; }
    
    public int LeftGridXCoord => XCoord * Dims;
    public int BottomGridYCoord => YCoord * Dims;

    private List<GridTile> _tileList;

    private Dungeon _dungeon;

    public GridTile[] GetTiles()
    {
        GridTile[] tiles = GetComponentsInChildren<GridTile>();
        if (tiles.Length == 0)
        {
            // Rooms were built and tiles transferred; find them in the map
            _tileList = new List<GridTile>();
            Utils.DoForXY(Dims, Dims, (x, y) =>
            {
                GridTile tile = GridTileFromLocalCoord(x, y);
                if (tile != null)
                {
                    _tileList.Add(tile);
                }
            });

            return _tileList.ToArray();
        }
        
        return tiles;
    }

    private GridTile GridTileFromLocalCoord(int x, int y)
    {
        int currX = LeftGridXCoord + x;
        int currY = BottomGridYCoord + y;
        GridTile tile = _dungeon.Grid.Get(currX, currY).Tile;
        return tile;
    }

    public bool HasConnectorToDirection(Direction direction)
    {
        GridTile[] tiles = GetTiles();
        return tiles.Any(a => a.IsConnectorTile() && a.ConnectsTo == direction);
    }

    public bool HasExactMatchingConnector(GridTile connector)
    {
        return GetExactMatchingConnector(connector) != null;
    }

    public List<ConnectorInfo> GetConnectorInfo()
    {
        var connectors = new List<ConnectorInfo>();
        var tiles = GetTiles();
        foreach (var tile in tiles)
        {
            if (tile.ConnectsTo == Direction.Left)
            {
                connectors.Add(new ConnectorInfo(tile.LocalY, Direction.Left));
            }

            if (tile.ConnectsTo == Direction.Right)
            {
                connectors.Add(new ConnectorInfo(tile.LocalY, Direction.Right));
            }

            if (tile.ConnectsTo == Direction.Down)
            {
                connectors.Add(new ConnectorInfo(tile.LocalX, Direction.Down));
            }

            if (tile.ConnectsTo == Direction.Up)
            {
                connectors.Add(new ConnectorInfo(tile.LocalX, Direction.Up));
            }
        }

        return connectors;
    }
    

    public GridTile GetExactMatchingConnector(GridTile connector)
    {
        if (connector.ConnectsTo == null)
        {
            return null;
        }

        Direction direction = Utils.GetOppositeDirection(connector.ConnectsTo.Value);
        GridTile[] tiles = GetTiles();
        if (direction == Direction.Down || direction == Direction.Up)
        {
            return tiles.FirstOrDefault(a => a.IsConnectorTile() && a.ConnectsTo == direction &&
                (int)a.transform.localPosition.x == (int)connector.transform.localPosition.x);
        }

        return tiles.FirstOrDefault(a => a.IsConnectorTile() && a.ConnectsTo == direction &&
            (int)a.transform.localPosition.y == (int)connector.transform.localPosition.y);
    }

    public void InitRoom(Dungeon dungeon, GridTile genericGridTileTemplate)
    {
        _dungeon = dungeon;

        // TODO: we can probably make this 100x more efficient; tons of useless operations here... just 
        // take the props directly from the Tilemap fields set above!
        foreach (Tilemap tilemap in GetComponentsInChildren<Tilemap>())
        {
            for (var x = tilemap.cellBounds.x; x < tilemap.cellBounds.xMax; x++)
            {
                for (var y = tilemap.cellBounds.y; y < tilemap.cellBounds.yMax; y++)
                {
                    // Test rule tile
                    RuleTile ruleTile = tilemap.GetTile<RuleTile>(new Vector3Int(x, y, 0));
                    if (ruleTile != null)
                    {
                        var gridTile = ruleTile.InstantiateGridTile(genericGridTileTemplate, this);
                        gridTile.transform.localPosition = new Vector3(x + 1, y + 1); // TODO: why do they need + 1...?
                        gridTile.Show(false);
                    }
                }
            }
        }

        foreach (GridTile tile in GetTiles())
        {
            tile.CachedRoom = this;
            if (tile.GetComponent<SpriteRenderer>() != null)
            {
                tile.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    public void PopulateProps()
    {
        if (Entities != null)
        {
            Utils.DoForXY(Entities.cellBounds.xMax, Entities.cellBounds.yMax, (x, y) =>
            {
                PropTile propTile = Entities.GetTile<PropTile>(new Vector3Int(x, y, 0));
                if (propTile != null)
                {
                    // TODO: why do we need the + 1...?
                    var tile = GridTileFromLocalCoord(x + 1, y + 1);
                    propTile.RollSpawn(_dungeon, tile);
                }
            });

            Entities.gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        if (Entities == null)
        {
            // TODO: better to make a special Script for these, and for that script to also have special behavior
            Entities = GetComponentsInChildren<Tilemap>(true).FirstOrDefault(a => a.name == "Entities");
        }
    }

    [UsedImplicitly]
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        var pos = transform.position;
        Gizmos.DrawLine(pos.OffsetBy(0.5f), (pos + (transform.up * Dims)).OffsetBy(0.5f));
        Gizmos.DrawLine(pos.OffsetBy(0.5f), (pos + (transform.right * Dims)).OffsetBy(0.5f));
        Gizmos.DrawLine((pos + (transform.up * Dims)).OffsetBy(0.5f), (pos + (transform.up * Dims + transform.right * Dims)).OffsetBy(0.5f));
        Gizmos.DrawLine((pos + (transform.right * Dims)).OffsetBy(0.5f), (pos + (transform.up * Dims + transform.right * Dims)).OffsetBy(0.5f));
    }
}

public struct ConnectorInfo
{
    public ConnectorInfo(int pos, Direction dir)
    {
        LocalPos = pos;
        Direction = dir;
    }

    public int LocalPos;
    public Direction Direction; 
}