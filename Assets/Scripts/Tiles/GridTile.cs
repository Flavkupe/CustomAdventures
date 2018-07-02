using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridTile : MonoBehaviour, IHasCoords
{
    public int XCoord { get; set; }
    public int YCoord { get; set; }

    public Tilemap[] UnhideOnDisable;   
       
    public bool IsConnectorNeighbor = false;
    
    public bool IsReserved { get; set; }

    public Direction? ConnectsTo
    {
        get
        {
            Room room = GetRoom();

            if ((int)transform.localPosition.x == 0)
            {
                return Direction.Left;
            }            
            if ((int)transform.localPosition.y == 0)
            {
                return Direction.Down;
            }
            if ((int)transform.localPosition.x == room.Dims - 1)
            {
                return Direction.Right;
            }
            if ((int)transform.localPosition.y == room.Dims - 1)
            {
                return Direction.Up;
            }

            return null;
        }
    }    

    public int LocalX { get { return (int)transform.localPosition.x; } }
    public int LocalY { get { return (int)transform.localPosition.y; } }

    public Room CachedRoom;
    public Room GetRoom()
    {
        if (CachedRoom != null)
        {
            return CachedRoom;
        }

        if (transform.GetComponentsInParent<Room>(true).Length == 0)
        {
            Debug.Assert(false, "No room associated with this tile");
        }

        CachedRoom = transform.GetComponentsInParent<Room>(true).FirstOrDefault();
        return CachedRoom;
    }

    public bool CanOccupy(OccupancyRule rule = OccupancyRule.MustBeEmpty)
    {
        return Game.Dungeon.Grid.CanOccupy(this, rule);
    }

    public bool IsConnectorTile()
    {        
        Room room = GetRoom();

        if ((int)transform.localPosition.x == 0 ||
            (int)transform.localPosition.y == 0 ||
            (int)transform.localPosition.x == room.Dims - 1 ||
            (int)transform.localPosition.y == room.Dims - 1)
        {
            return true;
        }

        return false;
    }

    public void Show(bool show)
    {
        if (GetComponent<SpriteRenderer>() == null)
        {
            Debug.Assert(false, "Trying to show tile with no sprite renderer");
            return;
        }

        GetComponent<SpriteRenderer>().enabled = show;        
    }

    public TileEntity GetTileEntity()
    {
        var contents = Game.Dungeon.Grid.Get(XCoord, YCoord);
        return contents.TileObject;
    }

    public List<PassableTileEntity> GetPassableTileEntities()
    {
        var contents = Game.Dungeon.Grid.Get(XCoord, YCoord);
        return contents.PassableEntities;
    }

    public Inventory<PassableTileItem> GetTileItems()
    {
        var contents = Game.Dungeon.Grid.Get(XCoord, YCoord);
        return contents.TileItems;
    }

    public void RemoveConnector()
    {
        if (UnhideOnDisable != null) {
            foreach (var tilemap in UnhideOnDisable)
            {
                tilemap.gameObject.SetActive(true);
            }                
        }

        Destroy(gameObject);
    }
}
