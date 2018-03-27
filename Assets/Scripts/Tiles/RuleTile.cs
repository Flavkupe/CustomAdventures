using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace UnityEngine
{
	[Serializable]
    [CreateAssetMenu(fileName = "Tile", menuName = "CreateAction Tiles/Rule Tile", order = 1)]
    public class RuleTile : Tile
	{
        public bool IsConnector = false;
        public bool IsPassable = true;

        public GridTile InstantiateGridTile(GridTile template, Room parentRoom)
        {
            if (template != null)
            {
                var gridTile = Instantiate(template);
                gridTile.transform.SetParent(parentRoom.transform);                    
                gridTile.IsConnectorNeighbor = IsConnector && IsPassable;
                return gridTile;
            }

            return null;
        }
    }
}
