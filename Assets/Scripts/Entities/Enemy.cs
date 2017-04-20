using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : TileEntity, IObjectOnTile, IDungeonActor
{
    public EnemyCardData Data { get; set; }

    public virtual void MoveAfterPlayer()
    {
        // Basic move
        if (Player.Instance.XCoord > this.XCoord)
        {
            this.TryMove(Direction.Right);
        }
        else if (Player.Instance.XCoord < this.XCoord)
        {
            this.TryMove(Direction.Left);
        }
        else if (Player.Instance.YCoord > this.YCoord)
        {
            this.TryMove(Direction.Up);
        }
        else if (Player.Instance.YCoord < this.YCoord)
        {
            this.TryMove(Direction.Down);
        }
    }

    protected override void OnClicked()
    {
        UIManager.Instance.UpdateEntityPanel(this);
        base.OnClicked();
    }

    // Use this for initialization
    void Start ()
    {		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(int damage)
    {
        this.Data.HP -= damage;
        this.ShowFloatyText(damage.ToString());
        if (this.Data.HP <= 0)
        {
            this.Die();
        }
    }

    private void Die()
    {
        DungeonManager.Instance.RemoveEnemy(this);
        Destroy(this.gameObject, 0.5f);
    }
}
