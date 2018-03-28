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

    public int NumDraws = 2;
    public bool BossArea = false;
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
                Game.CardDraw.PerformDungeonEvents(this);
            }
        }
    }

    public List<GridTile> GetAreaTiles()
    {
        List<GridTile> tiles = this.parentRoom.GetTiles().ToList();
        int firstX = this.parentRoom.LeftGridXCoord + (int)this.transform.localPosition.x;
        int firstY = this.parentRoom.BottomGridYCoord + (int)this.transform.localPosition.y;
        int lastX = firstX + (int)(this.col.size.x);
        int lastY = firstY + (int)(this.col.size.y);
        List<GridTile> areaTiles = tiles.Where(a => a.XCoord >= firstX && a.XCoord < lastX &&
                                                a.YCoord >= firstY && a.YCoord < lastY).ToList();
        return areaTiles;
    }

    /// <summary>
    /// Gets tiles which are corners (2 free neighbors, 2 filled ones)
    /// </summary>
    /// <returns></returns>
    public List<GridTile> GetCornerTiles()
    {
        List<GridTile> corners = new List<GridTile>();
        List<GridTile> tiles = this.GetAreaTiles();
        TileGrid grid = Game.Dungeon.Grid;
        foreach (GridTile tile in tiles)
        {
            if (grid.IsCorner(tile.XCoord, tile.YCoord))
            {
                corners.Add(tile);
            }
        }

        return corners;
    }

    public List<GridTile> GetWideOpenTiles()
    {
        List<GridTile> freeTiles = new List<GridTile>();
        TileGrid grid = Game.Dungeon.Grid;
        List<GridTile> tiles = this.GetAreaTiles().Where(a => grid.CanOccupy(a)).ToList();
        foreach (GridTile tile in tiles)
        {
            if (grid.GetAll8Neighbors(tile.XCoord, tile.YCoord).All(a => grid.CanOccupy(a, OccupancyRule.CanBeTemporaryEntity)))
            {
                freeTiles.Add(tile);
            }
        }

        return freeTiles;
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