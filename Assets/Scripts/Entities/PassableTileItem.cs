using System;
using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PassableTileItem : PassableTileEntity, IStackable
{
    public InventoryItem Item { get; set; }

    public override TileEntityType EntityType
    {
        get
        {
            return TileEntityType.Item;
        }
    }

    public int StackSize
    {
        get { return Item.StackSize; }
        set { Item.StackSize = value; }
    }    

    public int MaxStack { get { return Item.MaxStack; } }
    public int SpaceLeft { get { return Item.SpaceLeft; } }
    public string Identifier { get { return Item.Identifier; } }

    public override void RemoveFromGrid()
    {
        TileGrid grid = Game.Dungeon.Grid;
        grid.ClearPassableTileItem(this);
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

    public void DestroyItem()
    {
        Destroy(this.gameObject);
    }

    public override bool PlayerCanInteractWith()
    {
        return false;
    }

    public bool ItemCanStack(IStackable other)
    {
        return Item.ItemCanStack(other);
    }

    public void StackItems(IStackable other)
    {
        Item.StackItems(other);
    }
}
