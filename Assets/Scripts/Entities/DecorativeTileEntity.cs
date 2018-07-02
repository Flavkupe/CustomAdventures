using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DecorativeTileEntity : PassableTileEntity {

    [UsedImplicitly]
    private void Start()
    {
        GetComponent<SpriteRenderer>().sortingLayerName = "DecorativeTileEntities";
    }

    public void SetSprite(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public override TileEntityType EntityType
    {
        get { return TileEntityType.Environment; }
    }
}
