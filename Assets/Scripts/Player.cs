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

    void Start()
    {
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
        int damage = this.Stats.BaseStrength;
        if (this.Stats.Inventory.EquippedWeapon != null)
        {
            damage += this.Stats.Inventory.EquippedWeapon.Data.Power;
        }

        enemy.TakeDamage(damage);
    }

    public bool Unequip(InventoryItem item)
    {
        PlayerInventory inv = this.Stats.Inventory;
        if (this.TryMoveToInventory(item, false))
        {
            if (item.Type == InventoryItemType.Weapon)
            {
                inv.EquippedWeapon = null;
            }
            else if (item.Type == InventoryItemType.Armor)
            {
                inv.EquippedArmor = null;
            }
            else if (item.Type == InventoryItemType.Accessory)
            {
                inv.EquippedAccessory = null;
            }

            UIManager.Instance.UpdateInventory();
            UIManager.Instance.UpdateUI();
            return true;
        }
        
        return false;        
    }

    public void Equip(InventoryItem item)
    {
        PlayerInventory inv = this.Stats.Inventory;
        InventoryItem temp = null;
        if (item.Type == InventoryItemType.Weapon)
        {
            temp = inv.EquippedWeapon;
            inv.EquippedWeapon = item as InventoryItem<WeaponCardData>;
        }
        else if (item.Type == InventoryItemType.Armor)
        {
            // TODO
            temp = inv.EquippedArmor;
            inv.EquippedArmor = item as InventoryItem<WeaponCardData>;
        }
        else if (item.Type == InventoryItemType.Accessory)
        {
            // TODO
            temp = inv.EquippedAccessory;
            inv.EquippedAccessory = item as InventoryItem<WeaponCardData>; ;
        }

        if (inv.InventoryItems.Contains(item))
        {
            inv.InventoryItems.Remove(item);
        }

        this.TryMoveToInventory(temp, false);
        UIManager.Instance.UpdateInventory();
        UIManager.Instance.UpdateUI();
    }

    public bool TryMoveToInventory(InventoryItem item, bool updateUI)
    {
        if (item == null)
        {
            return false;
        }

        bool madeChanges = false;
        PlayerInventory inv = Stats.Inventory;
        foreach (InventoryItem current in inv.InventoryItems)
        {
            if (current.ItemCanStack(item))
            {
                current.StackItems(item);
                madeChanges = true;
                if (item.CurrentStackSize == 0)
                {
                    break;
                }                
            }
        }

        if (item.CurrentStackSize > 0 && inv.InventoryItems.Count < inv.MaxItems)
        {
            madeChanges = true;
            inv.InventoryItems.Add(item);
            if (updateUI)
            {
                UIManager.Instance.UpdateInventory();
            }            
        }

        if (madeChanges && updateUI)
        {            
            UIManager.Instance.UpdateInventory();
        }

        return madeChanges;
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
    public InventoryItem<WeaponCardData> EquippedWeapon;

    // TODO
    public InventoryItem<WeaponCardData> EquippedArmor;
    public InventoryItem<WeaponCardData> EquippedAccessory;

    public List<InventoryItem> InventoryItems = new List<InventoryItem>();

    public int MaxItems = 3;
}