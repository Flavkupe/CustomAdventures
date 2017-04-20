using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomArea : MonoBehaviour
{
    private Room parentRoom = null;
    private BoxCollider2D col;

    public OnPlayerEnterEvents OnPlayerEnter = OnPlayerEnterEvents.DungeonEvents;

    // Use this for initialization
    void Start ()
    {
        col = this.GetComponent<BoxCollider2D>();

        parentRoom = this.transform.GetComponentInParent<Room>();
        Debug.Assert(parentRoom != null, "RoomArea should be a child of a Room");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            if (this.OnPlayerEnter == OnPlayerEnterEvents.DungeonEvents)
            {
                DungeonManager.Instance.PerformDungeonEvents(this);
            }
        }
    }

    public List<Tile> GetAreaTiles()
    {
        List<Tile> tiles = this.parentRoom.GetTiles().ToList();
        int firstX = this.parentRoom.LeftGridXCoord + (int)this.transform.localPosition.x;
        int firstY = this.parentRoom.BottomGridYCoord + (int)this.transform.localPosition.y;
        int lastX = firstX + (int)(this.col.size.x);
        int lastY = firstY + (int)(this.col.size.y);
        List<Tile> areaTiles = tiles.Where(a => a.XCoord >= firstX && a.XCoord < lastX &&
                                                a.YCoord >= firstY && a.YCoord < lastY).ToList();
        return areaTiles;
    }

    // Update is called once per frame
    void Update ()
    {		
	}
}

public enum OnPlayerEnterEvents
{
    DungeonEvents,
    CustomEvents,
}