
using JetBrains.Annotations;
using UnityEngine;

public class DecorativeTileObject : TileEntity
{
    public DecorativePropData Data { get; set; }

    public override TileEntityType EntityType => TileEntityType.Environment;

    public override bool PlayerCanInteractWith()
    {
        return false;
    }

    [UsedImplicitly]
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Data.Sprite;
        GetComponent<SpriteRenderer>().sortingLayerName = "Entities";
    }
}

