using UnityEngine;

public abstract class PropData : ScriptableObject, IGeneratesTileEntity
{
    // [Tooltip("Out of 1.0: 0 never spawns, 1 always spawns.")]
    // public float SpawnChance = 1;

    public abstract TileEntity InstantiateTileEntity();
}

public abstract class PropData<TTileEntityType> 
    : PropData, IGeneratesTileEntity<TTileEntityType> where TTileEntityType : TileEntity

{
    public abstract TTileEntityType InstantiateEntity();

    public sealed override TileEntity InstantiateTileEntity()
    {
        return InstantiateEntity();
    }
}
