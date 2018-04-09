using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PassableTileItem : TileEntity
{
    public InventoryItem Item { get; set; }

    public override TileEntityType EntityType
    {
        get
        {
            return TileEntityType.Item;
        }
    }

    public override void RemoveFromGrid()
    {
        TileGrid grid = Game.Dungeon.Grid;
        grid.ClearPassableTileEntity(this);
    }

    [UsedImplicitly]
    private void Start ()
    {
        if (Item != null && Item.GetData<ItemCardData>() != null)
        {
            GetComponent<SpriteRenderer>().sprite = Item.GetData<ItemCardData>().Sprite;
        }

        GetComponent<SpriteRenderer>().sortingLayerName = "StackableTileEntities";
    }

    public override bool PlayerCanInteractWith()
    {
        return false;
    }
}
