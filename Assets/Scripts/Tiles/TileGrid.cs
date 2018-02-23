using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileGrid : MonoBehaviour
{
    [Serializable]
    public class TileContents
    {
        public Tile Tile;
        public TileEntity TileObject;
    }

    public TileContents[,] grid;

    public TileContents Get(int x, int y)
    {
        return grid[x, y];
    }

    public Tile GetTile(int x, int y)
    {
        return grid[x, y] == null ? null : grid[x, y].Tile;
    }

    public IEnumerable<Tile> GetAllTiles()
    {
        List<Tile> list = new List<Tile>();
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

    public void PutTile(Tile tile)
    {
        this.PutTile(tile.XCoord, tile.YCoord, tile);
    }

    public void PutTile(int x, int y, Tile tile)
    {
        grid[x, y].Tile = tile;
        tile.XCoord = x;
        tile.YCoord = y;
        tile.transform.parent = this.transform;
    }

    public List<Tile> GetNeighbors(int x, int y)
    {
        List<Tile> neighbors = new List<Tile>();
        if (!this.IsOffBounds(x - 1, y)) { neighbors.Add(this.grid[x - 1, y].Tile); }
        if (!this.IsOffBounds(x + 1, y)) { neighbors.Add(this.grid[x + 1, y].Tile); }
        if (!this.IsOffBounds(x, y - 1)) { neighbors.Add(this.grid[x, y - 1].Tile); }
        if (!this.IsOffBounds(x, y + 1)) { neighbors.Add(this.grid[x, y + 1].Tile); }
        return neighbors;
    }

    public List<Tile> GetCornerNeighbors(int x, int y)
    {
        List<Tile> neighbors = new List<Tile>();
        if (!this.IsOffBounds(x - 1, y - 1)) { neighbors.Add(this.grid[x - 1, y - 1].Tile); }
        if (!this.IsOffBounds(x + 1, y + 1)) { neighbors.Add(this.grid[x + 1, y + 1].Tile); }
        if (!this.IsOffBounds(x + 1, y - 1)) { neighbors.Add(this.grid[x + 1, y - 1].Tile); }
        if (!this.IsOffBounds(x - 1, y + 1)) { neighbors.Add(this.grid[x - 1, y + 1].Tile); }
        return neighbors;
    }

    public List<Tile> GetAll8Neighbors(int x, int y)
    {
        List<Tile> neighbors = this.GetNeighbors(x, y);
        neighbors.AddRange(this.GetCornerNeighbors(x, y));
        return neighbors;
    }

    public void ClearTile(int x, int y)
    {
        grid[x, y].TileObject = null;
    }

    public void PutObject<T>(Tile tile, T obj, bool moveObj = false) where T : TileEntity
    {
        this.PutObject(tile.XCoord, tile.YCoord, obj, moveObj);
    }

    public void PutObject<T>(int x, int y, T obj, bool moveObj = false) where T : TileEntity
    {        
        grid[x, y].TileObject = obj;
        obj.XCoord = x;
        obj.YCoord = y;
        if (moveObj)
        {
            Tile tile = grid[x, y].Tile;
            obj.transform.position = tile.transform.position;
        }
    }

    public TileContents GetAdjacent(int x, int y, Direction direction)
    {
        return this.grid.GetAdjacent(x, y, direction);
    }

    public TileEntity GetAdjacentObject(int x, int y, Direction direction)
    {
        TileContents contents = this.GetAdjacent(x, y, direction);
        return contents == null ? null : contents.TileObject;
    }

    public bool IsOffBounds(int x, int y)
    {
        return this.grid.IsOffBounds(x, y);
    }    

    public void MoveTo<T>(int x, int y, Direction direction, T obj) where T : TileEntity
    {
        TileContents contents = this.GetAdjacent(x, y, direction);
        Debug.Assert(this.CanOccupyContents(contents), "Trying to occupy tile that cannot be occupied!");
        this.ClearTile(x, y);
        this.PutObject(contents.Tile.XCoord, contents.Tile.YCoord, obj);
    }

    public bool CanOccupyAdjacent(int x, int y, Direction direction, OccupancyRule rule = OccupancyRule.MustBeEmpty)
    {
        TileContents contents = this.GetAdjacent(x, y, direction);
        return this.CanOccupyContents(contents, rule);
    }

    public bool IsCorner(int x, int y, OccupancyRule rule = OccupancyRule.CanBeTemporaryEntity)
    {
        if (!this.CanOccupy(x, y, OccupancyRule.MustBeEmpty)) return false;
        bool top = this.CanOccupyAdjacent(x, y, Direction.Up, rule);
        bool left = this.CanOccupyAdjacent(x, y, Direction.Left, rule);
        bool right = this.CanOccupyAdjacent(x, y, Direction.Right, rule);
        bool bottom = this.CanOccupyAdjacent(x, y, Direction.Down, rule);
        if ((top && bottom) || (left && right)) return false;
        if ((top && right) || (top && left)) return true;
        if ((bottom && right) || (bottom && left)) return true;
        return false;        
    }

    public bool CanOccupy(Tile tile, OccupancyRule rule = OccupancyRule.MustBeEmpty)
    {
        return tile != null && CanOccupy(tile.XCoord, tile.YCoord, rule);
    }

    /// <summary>
    /// Get all entities of type filter (or all for null) within radius "range" of center point x,y.
    /// </summary>
    public List<TileEntity> GetRadialEntities(int x, int y, int range, TileEntityType? filter = null)
    {
        List<TileEntity> entities = new List<TileEntity>();
        List<TileContents> contents = this.GetRadialTileContents(x, y, range);
        entities = contents.Where(a => a.TileObject != null).Select(b => b.TileObject).ToList();
        if (filter != null)
        {
            entities = entities.Where(a => Utils.HasFlag((int)a.EntityType, (int)filter)).ToList();
        }

        return entities;
    }

    public List<TileContents> GetRadialTileContents(int x, int y, int range)
    {
        List<TileContents> tilesInArea = new List<TileContents>();
        Utils.DoFromXYToXY(x - range, y - range, x + range, y + range, (currX, currY) =>
        {
            if (!IsOffBounds(currX, currY))
            {
                if (Vector2.Distance(new Vector2(x, y), new Vector2(currX, currY)) <= (float)range)
                {
                    TileContents contents = Get(currX, currY);
                    if (contents != null)
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
        if (this.IsOffBounds(x, y))
        {
            return false;
        }

        TileContents contents = this.grid[x, y];
        if (!this.CanOccupyContents(contents, rule))
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
            if (contents.TileObject == Player.Instance && FitsOccupancyRule(rule, OccupancyRule.CanBePlayer))
            {
                return true;
            }

            return false;
        }

        return true;
    }

    // Use this for initialization
    void Start ()
    {		
	}

	// Update is called once per frame
	void Update () {
		
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

public interface IDungeonActor
{
    IEnumerator ProcessCharacterTurn();
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
