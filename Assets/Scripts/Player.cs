using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : TileEntity
{
    private static Player instance;

    public static Player Instance { get { return instance; } private set { instance = value; } }

    public PlayerStats Stats;

    // Use this for initialization
    void Awake()
    {
        Instance = this;        
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (GameManager.Instance.IsPaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            this.PlayerMoveCommand(Direction.Up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            this.PlayerMoveCommand(Direction.Down);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            this.PlayerMoveCommand(Direction.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            this.PlayerMoveCommand(Direction.Right);
        }
    }

    public void PlayerMoveCommand(Direction direction)
    {
        TileGrid grid = DungeonManager.Instance.Grid;
        if (grid.CanOccupyAdjacent(this.XCoord, this.YCoord, direction))
        {
            this.TryMove(direction);
        }
        else
        {
            TileEntity obj = grid.GetAdjacentObject(this.XCoord, this.YCoord, direction);
            if (obj != null)
            {
                // Tile entity occupies spot
                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Enemy occupies spot
                    this.AttackEnemy(enemy);
                }
            }
            else
            {
                // Boundry
            }            
        }

        DungeonManager.Instance.AfterPlayerMove();
        UIManager.Instance.UpdateUI();
    }

    private void AttackEnemy(Enemy enemy)
    {
        enemy.TakeDamage(3);    
    }
}

[Serializable]
public class PlayerStats
{
    public int Level = 1;
    public int HP = 10;
    public int Energy = 0;
    public int BaseStrength = 1;

    public PlayerInventory Inventory;
}

[Serializable]
public class PlayerInventory
{
    public Weapon EquippedWeapon;

    public InventoryItem[] InventoryItems;
}