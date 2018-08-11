using System.Collections;
using System.Collections.Generic;

public delegate IEnumerator ActionOnEntities(List<TileEntity> entities);

public class EntitySelectionOptions
{
    public int NumToSelect;
    public List<TileEntity> Filter;

    public EntitySelectionOptions(int numToSelect, List<TileEntity> filter)
    {
        NumToSelect = numToSelect;
        Filter = filter;
    }
}