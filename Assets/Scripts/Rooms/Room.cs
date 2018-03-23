using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour, IHasCoords
{
    /// <summary>
    /// The NxN dimensions for the rooms.
    /// </summary>
    public int Dims = 9;

    public int XCoord { get; set; }
    public int YCoord { get; set; }      
    
    public int LeftGridXCoord { get { return this.XCoord * Dims; } }
    public int BottomGridYCoord { get { return this.YCoord * Dims; } }

    public GridTile[] GetTiles()
    {
        GridTile[] tiles = this.GetComponentsInChildren<GridTile>();
        if (tiles.Length == 0)
        {
            // Rooms were built and tiles transferred; find them in the map
            List<GridTile> tileList = new List<GridTile>();
            Utils.DoForXY(Dims, Dims, (x, y) =>
            {
                int currX = this.LeftGridXCoord + x;
                int currY = this.BottomGridYCoord + y;
                GridTile tile = Game.Dungeon.Grid.Get(currX, currY).Tile;
                if (tile != null)
                {
                    tileList.Add(tile);
                }
            });

            return tileList.ToArray();
        }
        else
        {
            return tiles;
        }        
    }

    public bool HasConnectorToDirection(Direction direction)
    {
        GridTile[] tiles = this.GetTiles();
        return tiles.Any(a => a.IsConnectorTile() && a.ConnectsTo == direction);
    }

    public bool HasExactMatchingConnector(GridTile connector)
    {
        Direction direction = Utils.GetOppositeDirection(connector.ConnectsTo.Value);
        GridTile[] tiles = this.GetTiles();
        if (direction == Direction.Down || direction == Direction.Up)
        {
            return tiles.Any(a => a.IsConnectorTile() && a.ConnectsTo == direction && 
                a.transform.localPosition.x == connector.transform.localPosition.x);
        }
        else
        {
            return tiles.Any(a => a.IsConnectorTile() && a.ConnectsTo == direction &&
                a.transform.localPosition.y == connector.transform.localPosition.y);
        }
    }

    public void InitRoomTiles()
    {
        foreach(Tilemap tilemap in this.GetComponentsInChildren<Tilemap>())
        {
            for (var x = tilemap.cellBounds.x; x < tilemap.cellBounds.xMax; x++)
            {
                for (var y = tilemap.cellBounds.y; y < tilemap.cellBounds.yMax; y++)
                {
                    RuleTile tile = tilemap.GetTile<RuleTile>(new Vector3Int(x, y, 0));
                    if (tile != null)
                    {
                        var gridTile = tile.InstantiateGridTile(Game.Dungeon.GenericTileTemplate, this);
                        gridTile.transform.localPosition = new Vector3(x + 1, y + 1); // TODO: why do they need + 1...?
                        tile.sprite = null;
                    }
                }
            }
        }

        foreach (GridTile tile in this.GetTiles())
        {
            tile.CachedRoom = this;
            if (tile.GetComponent<SpriteRenderer>() != null)
            {
                tile.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 pos = this.transform.position;
        Gizmos.DrawLine(pos.OffsetBy(0.5f), (pos + (this.transform.up * this.Dims)).OffsetBy(0.5f));
        Gizmos.DrawLine(pos.OffsetBy(0.5f), (pos + (this.transform.right * this.Dims)).OffsetBy(0.5f));
        Gizmos.DrawLine((pos + (this.transform.up * this.Dims)).OffsetBy(0.5f), (pos + (this.transform.up * this.Dims + this.transform.right * this.Dims)).OffsetBy(0.5f));
        Gizmos.DrawLine((pos + (this.transform.right * this.Dims)).OffsetBy(0.5f), (pos + (this.transform.up * this.Dims + this.transform.right * this.Dims)).OffsetBy(0.5f));
    }
}
