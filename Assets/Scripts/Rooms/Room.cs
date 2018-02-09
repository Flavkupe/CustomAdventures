using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public Tile[] GetTiles()
    {
        Tile[] tiles = this.GetComponentsInChildren<Tile>();
        if (tiles.Length == 0)
        {
            // Rooms were built and tiles transferred; find them in the map
            List<Tile> tileList = new List<Tile>();
            Utils.DoForXY(Dims, Dims, (x, y) =>
            {
                int currX = this.LeftGridXCoord + x;
                int currY = this.BottomGridYCoord + y;
                Tile tile = DungeonManager.Instance.Grid.Get(currX, currY).Tile;
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
        Tile[] tiles = this.GetTiles();
        return tiles.Any(a => a.IsConnectorTile() && a.ConnectsTo == direction);
    }

    public bool HasExactMatchingConnector(Tile connector)
    {
        Direction direction = Utils.GetOppositeDirection(connector.ConnectsTo.Value);
        Tile[] tiles = this.GetTiles();
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
        foreach (Tile tile in this.GetTiles())
        {
            tile.CachedRoom = this;
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
