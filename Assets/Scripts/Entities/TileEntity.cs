﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class TileEntity : MonoBehaviourEx, IObjectOnTile
{
    public float TileSlideSpeed = 10.0f;

    public int XCoord { get; set; }
    public int YCoord { get; set; }

    public bool Selected { get; set; }

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

    public virtual PlayerInteraction GetPlayerInteraction(Player player)
    {
        return PlayerInteraction.None;
    }

    public virtual IEnumerator PlayerInteractWith()
    {
        yield return null;
    }

    public bool CanMove(Direction direction)
    {
        TileGrid grid = Game.Dungeon.Grid;
        return grid.CanOccupyAdjacent(this.XCoord, this.YCoord, direction);
    }

    public IEnumerator TryMove(Direction direction)
    {
        if (!CanMove(direction))
        {
            yield break;
        }

        Game.States.SetState(GameState.CharacterMoving);
        TileGrid grid = Game.Dungeon.Grid;
        grid.MoveTo(this.XCoord, this.YCoord, direction, this);
        GridTile newTile = grid.GetTile(this.XCoord, this.YCoord);
        yield return this.transform.MoveToSpotCoroutine(newTile.transform.position, this.TileSlideSpeed, false);
        Game.States.RevertState();
    }

    private void OnMouseUp()
    {
        if (Game.States.State == GameState.AwaitingSelection)
        {
            this.Selected = !this.Selected;
            Game.Dungeon.AfterToggledSelection(this);
        }
    }
}

[Flags]
public enum TileEntityType
{
    Player = 1,
    Enemy = 2,
    Environment = 4,
}
