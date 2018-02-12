using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class Treasure : TileEntity, IObjectOnTile
{
    public TreasureCardData Data { get; set; }

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
}
