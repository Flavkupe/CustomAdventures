using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

namespace UnityEngine
{
	[Serializable]
    public abstract class PropTile : Tile
	{
        [Tooltip("0.0 never spawns, 1.0 spawns all the time.")]
	    public float SpawnChance = 1.0f;

        [AssetIcon]
        public Sprite AssetIcon;

	    public abstract List<IGeneratesTileEntity> GetPossibleSpawns();

        public void RollSpawn(DungeonManager dungeon, GridTile tile)
        {
            if (tile.CanOccupy() && SpawnChance >= 1.0f || Random.Range(0.0f, 1.0f) <= SpawnChance)
            {
                var spawns = GetPossibleSpawns().Where(a => a != null).ToList();
                if (spawns.Count == 0)
                {
                    return;
                }

                var spawn = spawns.GetRandom();
                var entity = spawn.InstantiateTileEntity();
                entity.SpawnOnGrid(dungeon, tile);
            }
        }
    }
}
