
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TileTrap : PassableTileEntity
{
    public override TileEntityType EntityType { get { return TileEntityType.Trap; } }

    public TrapCardData Data { get; set; }

    [UsedImplicitly]
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Data.Sprite;
        GetComponent<SpriteRenderer>().sortingLayerName = "PassableTileEntities";
    }

    public override IEnumerator ProcessEntitySteppedOver(TileEntity other)
    {
        other.DoDamage(Data.Damage);
        yield return null;
    }

    public override void SpawnOnGrid(Dungeon dungeon, GridTile tile)
    {
        dungeon.SpawnPassableEntity(this, tile);
    }
}
