﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tile : MonoBehaviour, IHasCoords
{
    public int XCoord { get; set; }
    public int YCoord { get; set; }
    
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

    // Use this for initialization
    void Start ()
    {	
	}
	
	// Update is called once per frame
	void Update ()
    {		
	}
}