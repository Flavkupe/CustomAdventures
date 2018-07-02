using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine
{
	[Serializable]
    [CreateAssetMenu(fileName = "Tile", menuName = "Create Tiles/Static Prop Tile", order = 1)]
    public class CardPropTile : PropTile
	{
	    public PropData[] PossibleSpawns;

        public override List<IGeneratesTileEntity> GetPossibleSpawns()
        {
            return PossibleSpawns.Cast<IGeneratesTileEntity>().ToList();
        }
    }
}
