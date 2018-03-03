using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class Treasure : TileEntity, IObjectOnTile
{
    public TreasureCardData Data { get; set; }

    public override TileEntityType EntityType
    {
        get
        {
            return TileEntityType.Environment;
        }
    }

    private bool _canInteractWith = true;

    protected override void OnClicked()
    {
        //UIManager.Instance.UpdateEntityPanel(this);
        base.OnClicked();
    }

    // Use this for initialization
    void Start ()
    {
        this.GetComponent<SpriteRenderer>().sprite = this.Data.Sprite;
        this.GetComponent<SpriteRenderer>().sortingLayerName = "Entities";
    }
	
	// Update is called once per frame
	void Update () {
	}

    public override bool PlayerCanInteractWith()
    {
        return _canInteractWith;
    }

    public override PlayerInteraction PlayerInteractWith(Player player)
    {
        Game.CardDraw.PerformLootCardDrawing(2);
        this._canInteractWith = false;
        Destroy(this.gameObject, 1.0f);
        return PlayerInteraction.InteractWithObject;
    }
}
