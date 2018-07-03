using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileGrid : MonoBehaviour
{
    [Serializable]
    public class TileContents
    {
        public TileContents()
        {
            _tileItems.UnlimittedItems = true;
        }

        private Inventory<PassableTileItem> _tileItems = new Inventory<PassableTileItem>();
        private List<PassableTileEntity> _passableEntities = new List<PassableTileEntity>();

        public GridTile Tile;
        public TileEntity TileObject;
        public Inventory<PassableTileItem> TileItems => _tileItems;
        public List<PassableTileEntity> PassableEntities => _passableEntities;

        public bool HasTile => Tile != null;
        public bool IsPassable => HasTile && TileObject == null;
        public bool IsEmpty => HasTile && TileObject == null && _tileItems.Count == 0;
    }

    public TileContents[,] grid;

    public TileContents Get(int x, int y)
    {
        return grid[x, y];
    }

    public GridTile GetTile(int x, int y)
    {
        return grid[x, y] == null ? null : grid[x, y].Tile;
    }

    public IEnumerable<GridTile> GetAllTiles()
    {
        List<GridTile> list = new List<GridTile>();
        foreach (TileContents tile in grid)
        {
            if (tile != null && tile.Tile != null)
            {
                list.Add(tile.Tile);
            }
        }

        return list;
    }

    public void UnreserveAll()
    {
        foreach (TileContents tile in grid)
        {
            if (tile != null && tile.Tile != null)
            {
                tile.Tile.IsReserved = false;
            }
        }
    }

    public void Init(int dimX, int dimY)
    {
        grid = new TileContents[dimX, dimY];
        Utils.DoForXY(dimX, dimY, (x, y) =>
        {
            grid[x, y] = new TileContents();
        });            
    }

    public void PutTile(GridTile tile)
    {
        PutTile(tile.XCoord, tile.YCoord, tile);
    }

    public void PutTile(int x, int y, GridTile tile)
    {
        grid[x, y].Tile = tile;
        tile.XCoord = x;
        tile.YCoord = y;
        tile.transform.parent = transform;
    }

    public List<GridTile> GetNeighbors(int x, int y)
    {
        List<GridTile> neighbors = new List<GridTile>();
        if (!IsOffBounds(x - 1, y)) { neighbors.Add(grid[x - 1, y].Tile); }
        if (!IsOffBounds(x + 1, y)) { neighbors.Add(grid[x + 1, y].Tile); }
        if (!IsOffBounds(x, y - 1)) { neighbors.Add(grid[x, y - 1].Tile); }
        if (!IsOffBounds(x, y + 1)) { neighbors.Add(grid[x, y + 1].Tile); }
        return neighbors;
    }

    public List<GridTile> GetCornerNeighbors(int x, int y)
    {
        List<GridTile> neighbors = new List<GridTile>();
        if (!IsOffBounds(x - 1, y - 1)) { neighbors.Add(grid[x - 1, y - 1].Tile); }
        if (!IsOffBounds(x + 1, y + 1)) { neighbors.Add(grid[x + 1, y + 1].Tile); }
        if (!IsOffBounds(x + 1, y - 1)) { neighbors.Add(grid[x + 1, y - 1].Tile); }
        if (!IsOffBounds(x - 1, y + 1)) { neighbors.Add(grid[x - 1, y + 1].Tile); }
        return neighbors;
    }

    public List<GridTile> GetAll8Neighbors(int x, int y)
    {
        List<GridTile> neighbors = GetNeighbors(x, y);
        neighbors.AddRange(GetCornerNeighbors(x, y));
        return neighbors;
    }

    public void ClearTileEntity(int x, int y)
    {
        grid[x, y].TileObject = null;
        grid[x, y].Tile.IsReserved = false;
    }

    public void ClearPassableTileItem(PassableTileItem entity)
    {
        var objects = grid[entity.XCoord, entity.YCoord].TileItems;
        if (objects.TryRemoveItem(entity))
        {
            Destroy(entity.gameObject);
        }
    }

    public void ClearPassableTileEntitiy(PassableTileEntity entity)
    {
        var objects = grid[entity.XCoord, entity.YCoord].PassableEntities;
        if (objects.Contains(entity))
        {
            objects.Remove(entity);
            Destroy(entity.gameObject);
        }
    }

    public void PutObject<T>(GridTile tile, T obj, bool moveObj = false) where T : TileEntity
    {
        PutObject(tile.XCoord, tile.YCoord, obj, moveObj);
    }

    public void PutObject<T>(int x, int y, T obj, bool moveObj = false) where T : TileEntity
    {        
        grid[x, y].TileObject = obj;
        obj.XCoord = x;
        obj.YCoord = y;
        if (moveObj)
        {
            GridTile tile = grid[x, y].Tile;
            obj.transform.position = tile.transform.position;
            tile.IsReserved = false;
        }
    }

    public void PutPassableEntity(int x, int y, PassableTileEntity obj, bool moveObj = false)           
    {
        var objects = grid[x, y].PassableEntities;
        if (!objects.Contains(obj))
        {
            objects.Add(obj);
            if (moveObj)
            {
                GridTile tile = grid[x, y].Tile;
                obj.transform.position = tile.transform.position;
            }
        }
    }

    public void PutPassableItem<T>(int x, int y, T obj, bool moveObj = false) where T : PassableTileItem
    {
        if (grid[x, y].TileItems.TryAddItem(obj))
        {
            if (obj.StackSize > 0)
            {
                // Still some item left; add to tile
                obj.XCoord = x;
                obj.YCoord = y;
                if (moveObj)
                {
                    GridTile tile = grid[x, y].Tile;
                    obj.transform.position = tile.transform.position;
                }
            }
            else
            {
                // Item merged into other item; destroy this tile representation
                obj.DestroyItem();
            }
        }
        else
        {
            // TODO: what if we cannot add the item to the tile?
            obj.DestroyItem();
        }
    }

    public TileContents GetAdjacent(int x, int y, Direction direction)
    {
        return grid.GetAdjacent(x, y, direction);
    }

    public TileEntity GetAdjacentObject(int x, int y, Direction direction)
    {
        TileContents contents = GetAdjacent(x, y, direction);
        return contents?.TileObject;
    }

    public bool IsOffBounds(int x, int y)
    {
        return grid.IsOffBounds(x, y);
    }    

    public void MoveTo<T>(int x, int y, Direction direction, T obj) where T : TileEntity
    {
        TileContents contents = GetAdjacent(x, y, direction);
        Debug.Assert(CanOccupyContents(contents), "Trying to occupy tile that cannot be occupied!");
        ClearTileEntity(x, y);
        PutObject(contents.Tile.XCoord, contents.Tile.YCoord, obj);
    }

    public bool CanOccupyAdjacent(int x, int y, Direction direction, OccupancyRule rule = OccupancyRule.MustBeEmpty)
    {
        TileContents contents = GetAdjacent(x, y, direction);
        return CanOccupyContents(contents, rule);
    }

    public bool IsCorner(int x, int y, OccupancyRule rule = OccupancyRule.CanBeTemporaryEntity)
    {
        if (!CanOccupy(x, y, OccupancyRule.MustBeEmpty)) return false;
        bool top = CanOccupyAdjacent(x, y, Direction.Up, rule);
        bool left = CanOccupyAdjacent(x, y, Direction.Left, rule);
        bool right = CanOccupyAdjacent(x, y, Direction.Right, rule);
        bool bottom = CanOccupyAdjacent(x, y, Direction.Down, rule);
        if ((top && bottom) || (left && right)) return false;
        if ((top && right) || (top && left)) return true;
        if ((bottom && right) || (bottom && left)) return true;
        return false;        
    }

    public bool CanOccupy(GridTile tile, OccupancyRule rule = OccupancyRule.MustBeEmpty)
    {
        return tile != null && CanOccupy(tile.XCoord, tile.YCoord, rule);
    }

    /// <summary>
    /// Get all entities of type filter (or all for null) within radius "range" of center point x,y.
    /// </summary>
    public List<TileEntity> GetEntities(TileRangeType rangeType, int x, int y, int range, TileEntityType? filter = null)
    {
        List<TileContents> contents = null;
        switch (rangeType)
        {
            case TileRangeType.Radial:
                contents = GetRadialTileContents(x, y, range);
                break;
            case TileRangeType.Sides:
                contents = GetSideTileContents(x, y, range);
                break;
            default:
                throw new NotImplementedException();
        }

        var entities = contents.Where(a => a.TileObject != null).Select(b => b.TileObject).ToList();
        if (filter != null)
        {
            entities = entities.Where(a => Utils.HasFlag((int)a.EntityType, (int)filter)).ToList();
        }

        return entities;
    }

    public List<TileContents> GetSideTileContents(int x, int y, int range)
    {
        return GetTileContentsByCondition(x, y, range, (currX, currY) =>
        {
            // Adjacent to the sides (outwards)
            return (x == currX || y == currY) && !(x == currX && y == currY);
        });
    }

    public List<TileContents> GetRadialTileContents(int x, int y, int range)
    {
        return GetTileContentsByCondition(x, y, range, (currX, currY) =>
        {
            // Radial around center
            return Vector2.Distance(new Vector2(x, y), new Vector2(currX, currY)) <= (float)range;
        });
    }

    private List<TileContents> GetTileContentsByCondition(int x, int y, int range, Func<int, int, bool> condition)
    {
        List<TileContents> tilesInArea = new List<TileContents>();
        Utils.DoFromXYToXY(x - range, y - range, x + range, y + range, (currX, currY) =>
        {
            if (!IsOffBounds(currX, currY))
            {
                TileContents contents = Get(currX, currY);
                if (contents.HasTile && condition(currX, currY))                
                {
                    if (contents != null && contents.Tile != null)
                    {
                        tilesInArea.Add(contents);
                    }
                }
            }
        });

        return tilesInArea;
    }

    public bool CanOccupy(int x, int y, OccupancyRule rule = OccupancyRule.MustBeEmpty)
    {
        if (IsOffBounds(x, y))
        {
            return false;
        }

        TileContents contents = grid[x, y];
        if (!CanOccupyContents(contents, rule))
        {
            return false;
        }

        return true;
    }

    public bool CanOccupyContents(TileContents contents, OccupancyRule rule = OccupancyRule.MustBeEmpty)
    {
        if (contents == null || contents.Tile == null)
        {
            return false;
        }

        if (contents.Tile.IsReserved && !FitsOccupancyRule(rule, OccupancyRule.CanBeReserved))
        {
            return false;
        }

        if (contents.TileObject != null)
        {
            if (contents.TileObject == Game.Player && FitsOccupancyRule(rule, OccupancyRule.CanBePlayer))
            {
                return true;
            }

            return false;
        }

        return true;
    }

    private bool FitsOccupancyRule(OccupancyRule rule, OccupancyRule flag)
    {
        return Utils.HasFlag((int)rule, (int)OccupancyRule.CanBeReserved);
    }
}

[Flags]
public enum OccupancyRule
{
    MustBeEmpty = 1,
    CanBeReserved = 2,
    CanBePlayer = 4,
    CanBeTemporaryEntity = 6, // For checking adjacent spots to a spawn locale
}

public interface IObjectOnTile : IHasCoords
{
}

public interface IHasCoords
{
    int XCoord { get; set; }
    int YCoord { get; set; }
}

public enum Direction
{
    Up, Down, Left, Right
}

public enum TileRangeType
{
    Radial,
    Square,
    Sides,
    Diagonals,
}
