using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Linq;

namespace UnityEngine
{
	[Serializable]
    [CreateAssetMenu(fileName = "Tile", menuName = "Create Tiles/Card Prop Tile", order = 1)]
    public class StaticPropTile : PropTile
	{
	    public EntityCardData[] PossibleSpawns;

        public override List<IGeneratesTileEntity> GetPossibleSpawns()
        {
            return PossibleSpawns.Cast<IGeneratesTileEntity>().ToList();
        }
    }
}
