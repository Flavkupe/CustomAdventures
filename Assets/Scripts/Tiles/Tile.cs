using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class Tile : MonoBehaviour, IHasCoords
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
            Room room = this.GetRoom();

            if (this.transform.localPosition.x == 0)
            {
                return Direction.Left;
            }            
            if (this.transform.localPosition.y == 0)
            {
                return Direction.Down;
            }
            if (this.transform.localPosition.x == room.Dims - 1)
            {
                return Direction.Right;
            }
            if (this.transform.localPosition.y == room.Dims - 1)
            {
                return Direction.Up;
            }

            return null;
        }
    }    

    public Room CachedRoom = null;
    public Room GetRoom()
    {
        if (CachedRoom != null)
        {
            return CachedRoom;
        }

        if (this.transform.GetComponentsInParent<Room>(true).Length == 0)
        {
            Debug.Assert(false, "No room associated with this tile");
        }

        CachedRoom = this.transform.GetComponentsInParent<Room>(true).FirstOrDefault();
        return CachedRoom;
    }

    public bool CanOccupy(OccupancyRule rule = OccupancyRule.MustBeEmpty)
    {
        return DungeonManager.Instance.Grid.CanOccupy(this, rule);
    }

    public bool IsConnectorTile()
    {        
        Room room = this.GetRoom();

        if (this.transform.localPosition.x == 0 ||
            this.transform.localPosition.y == 0 ||
            this.transform.localPosition.x == room.Dims - 1 ||
            this.transform.localPosition.y == room.Dims - 1)
        {
            return true;
        }

        return false;
    }

    public void Show(bool show)
    {
        if (this.GetComponent<SpriteRenderer>() == null)
        {
            Debug.Assert(false, "Trying to show tile with no sprite renderer");
            return;
        }
        
        this.GetComponent<SpriteRenderer>().enabled = show;        
    }

    public TileEntity GetTileEntity()
    {
        var contents = Game.Dungeon.Grid.Get(this.XCoord, this.YCoord);
        return contents.TileObject;
    }

    // Use this for initialization
    void Start ()
    {	
	}
	
	// Update is called once per frame
	void Update ()
    {		
	}

    public void RemoveConnector()
    {
        if (UnhideOnDisable != null) {
            foreach (var tilemap in UnhideOnDisable)
            {
                tilemap.gameObject.SetActive(true);
            }                
        }

        Destroy(this.gameObject);
    }
}
