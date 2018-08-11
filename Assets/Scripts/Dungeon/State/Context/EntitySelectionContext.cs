using System.Collections;
using System.Collections.Generic;

public delegate IEnumerator ActionOnEntities(List<TileEntity> entities);

public class EntitySelectionOptions
{
    public int NumToSelect { get; }
    public List<TileEntity> Filter { get; }
    public List<GridTile> TileRange { get; }

    public EntitySelectionOptions(int numToSelect, List<TileEntity> filter, List<GridTile> tileRange)
    {
        NumToSelect = numToSelect;
        Filter = filter;
        TileRange = tileRange;
    }
}