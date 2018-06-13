using System;
using UnityEngine.Tilemaps;

namespace UnityEngine
{
	[Serializable]
    [CreateAssetMenu(fileName = "Tile", menuName = "Create Tiles/Prop Tile", order = 1)]
    public class PropTile : Tile
	{
        public EntityCardData[] PossibleSpawns;

        public void RollSpawn(TileGrid grid, GridTile tile)
        {
            if (tile.CanOccupy())
            {
                var spawn = PossibleSpawns.GetRandom().InstantiateTileEntity();
                grid.PutObject(tile, spawn, true);
            }
        }
    }
}
