﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Enemy : TileEntity, IObjectOnTile, IDungeonActor
{
    public EnemyCardData Data { get; set; }

    public override TileEntityType EntityType { get { return TileEntityType.Enemy; } }

    private int HP;

    public IEnumerator ProcessCharacterTurn()
    {
        if (Game.Dungeon.Grid.GetNeighbors(this.XCoord, this.YCoord).Where(t => t != null && t.GetTileEntity() != null)
                                                                    .Select(a => a.GetTileEntity()).Any(b => b == Game.Player))
        {
            yield return AttackPlayer();
            yield break;
        }

        // Basic move
        if (Game.Player.XCoord > this.XCoord)
        {
            yield return TryMove(Direction.Right);
        }
        else if (Game.Player.XCoord < this.XCoord)
        {
            yield return TryMove(Direction.Left);
        }
        else if (Game.Player.YCoord > this.YCoord)
        {
            yield return TryMove(Direction.Up);
        }
        else if (Game.Player.YCoord < this.YCoord)
        {
            yield return TryMove(Direction.Down);
        }
    }

    private IEnumerator AttackPlayer()
    {
        yield return new WaitForSecondsSpeedable(1.0f);
        Game.Player.TakeDamage(this.Data.Attack);
        yield return null;
    }

    public override void DoDamage(int damage)
    {
        this.TakeDamage(damage);
    }

    protected override void OnClicked()
    {
        Game.UI.UpdateEntityPanel(this);
        base.OnClicked();
    }

    // Use this for initialization
    void Start ()
    {
        this.HP = this.Data.MaxHP;
        this.GetComponent<SpriteRenderer>().sprite = this.Data.Sprite;
        this.GetComponent<SpriteRenderer>().sortingLayerName = "Entities";
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void TakeDamage(int damage)
    {
        if (this.HP > 0)
        {
            this.HP -= damage;
            this.ShowFloatyText(damage.ToString());
            if (this.HP <= 0)
            {
                this.Die();
            }
        }
    }

    private void Die()
    {
        Game.Dungeon.RemoveEnemy(this);
        Destroy(this.gameObject, 0.5f);
        Game.Player.GainXP(this.Data.EXP);
    }

    public override bool PlayerCanInteractWith()
    {
        return true;
    }

    public override PlayerInteraction PlayerInteractWith(Player player)
    {
        int damage = player.GetAttackStrength();
        this.TakeDamage(damage);
        return PlayerInteraction.Attack;
    }
}
