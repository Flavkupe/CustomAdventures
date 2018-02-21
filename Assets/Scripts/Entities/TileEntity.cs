﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class TileEntity : MonoBehaviour, IObjectOnTile
{
    public int XCoord { get; set; }
    public int YCoord { get; set; }

    public abstract TileEntityType EntityType { get; }

    public void ShowFloatyText(string text)
    {
        FloatyText damageText = Instantiate(TextManager.Instance.DamageTextTemplate);
        damageText.Init(this.transform.position, text);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    protected virtual void OnClicked()
    {
    }

    private void OnMouseDown()
    {
        this.OnClicked();
    }

    public virtual bool PlayerCanInteractWith()
    {
        return false;
    }

    public virtual void DoDamage(int damage)
    {
    }

    public virtual PlayerInteraction PlayerInteractWith(Player player)
    {
        return PlayerInteraction.None;
    }

    public bool TryMove(Direction direction)
    {
        TileGrid grid = DungeonManager.Instance.Grid;
        if (grid.CanOccupyAdjacent(this.XCoord, this.YCoord, direction))
        {
            grid.MoveTo(this.XCoord, this.YCoord, direction, this);
            Tile newTile = grid.GetTile(this.XCoord, this.YCoord);
            this.transform.position = newTile.transform.position;
            return true;
        }

        return false;
    }
}

[Flags]
public enum TileEntityType
{
    Player = 1,
    Enemy = 2,
    Environment = 4,
}
