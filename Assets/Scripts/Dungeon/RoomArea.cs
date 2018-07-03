using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomArea : MonoBehaviourEx
{
    private Room parentRoom;
    private BoxCollider2D col;

    public int NumDraws = 2;
    public bool IsBossArea => parentRoom.BossRoom;

    public bool IsEntranceArea => parentRoom.EntranceRoom;

    public event EventHandler<RoomArea> PlayerEnteredRoomArea;

    public bool IsNormalArea => parentRoom.IsNormalRoom;

    public bool CanMulligan => IsNormalArea;

    public bool RoomVisited { get; private set; }

    public OnPlayerEnterEvents OnPlayerEnter = OnPlayerEnterEvents.DungeonEvents;

    [UsedImplicitly]
    private void Start ()
    {
        col = GetComponent<BoxCollider2D>();

        parentRoom = transform.GetComponentInParent<Room>();
        Debug.Assert(parentRoom != null, "RoomArea should be a child of a Room");
    }


    [UsedImplicitly]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player == null)
        {
            return;
        }

        RoomVisited = true;

        if (OnPlayerEnter == OnPlayerEnterEvents.DungeonEvents)
        {
            DoDungeonEvents();
        }
    }

    private void DoDungeonEvents()
    {
        PlayerEnteredRoomArea?.Invoke(this, this);
        // Game.CardDraw.PerformDungeonEvents(this);
    }

    public List<GridTile> GetAreaTiles()
    {
        List<GridTile> tiles = parentRoom.GetTiles().ToList();
        int firstX = parentRoom.LeftGridXCoord + (int)transform.localPosition.x;
        int firstY = parentRoom.BottomGridYCoord + (int)transform.localPosition.y;
        int lastX = firstX + (int)(col.size.x);
        int lastY = firstY + (int)(col.size.y);
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
        List<GridTile> tiles = GetAreaTiles();
        TileGrid grid = parentRoom.Dungeon.Grid;
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
        TileGrid grid = parentRoom.Dungeon.Grid;
        List<GridTile> tiles = GetAreaTiles().Where(a => grid.CanOccupy(a)).ToList();
        foreach (GridTile tile in tiles)
        {
            if (grid.GetAll8Neighbors(tile.XCoord, tile.YCoord).All(a => grid.CanOccupy(a, OccupancyRule.CanBeTemporaryEntity)))
            {
                freeTiles.Add(tile);
            }
        }

        return freeTiles;
    }
}

public enum OnPlayerEnterEvents
{
    DungeonEvents,
    CustomEvents,
}