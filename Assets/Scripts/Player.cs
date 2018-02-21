using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : TileEntity
{
    private static Player instance;

    public override TileEntityType EntityType { get { return TileEntityType.Player; } }

    public static Player Instance { get { return instance; } private set { instance = value; } }

    public List<IAbilityCard> Abilities { get { return abilities; } }

    public PlayerStats Stats;

    private List<StatusEffect> Effects = new List<StatusEffect>();

    private List<IAbilityCard> abilities = new List<IAbilityCard>();

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    public void EquipAbility(IAbilityCard ability)
    {
        this.abilities.Add(ability);
        AbilityPanel.Instance.AddAbility(ability);
    }

    public void UseAbility(IAbilityCard ability)
    {
        // TODO: targetted abilities
        Debug.Assert(this.abilities.Contains(ability));
        ability.ActivateAbility();
        if (ability.ForgetOnUse)
        {
            this.abilities.Remove(ability);
        }
    }

    public void ApplyEffect(StatusEffect effect)
    {
        effect.Apply(this);
        Effects.Add(effect);
    }

    public void EffectExpire(StatusEffect effect)
    {
        effect.Expire(this);
        Effects.Remove(effect);
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

    private void ProcessEffects(EffectDurationType actionTaken)
    {
        foreach (StatusEffect effect in this.Effects.ToList())
        {
            if (effect.DurationType == actionTaken)
            {
                effect.Duration--;
                if (effect.Duration <= 0)
                {
                    this.EffectExpire(effect);
                }
            }
        }
    }

    private void OnAfterPlayerMove()
    {
        ProcessEffects(EffectDurationType.Steps);
    }

    private void OnAfterPlayerAttack()
    {
        ProcessEffects(EffectDurationType.Attacks);
    }

    public void PlayerMoveCommand(Direction direction)
    {
        TileGrid grid = DungeonManager.Instance.Grid;
        if (grid.CanOccupyAdjacent(this.XCoord, this.YCoord, direction))
        {
            if (this.TryMove(direction))
            {
                this.OnAfterPlayerMove();
            }
        }
        else
        {
            TileEntity obj = grid.GetAdjacentObject(this.XCoord, this.YCoord, direction);
            if (obj != null)
            {
                if (obj.PlayerCanInteractWith())
                {
                    switch(obj.PlayerInteractWith(this))
                    {
                        case PlayerInteraction.Attack:
                            this.OnAfterPlayerAttack();
                            break;
                        default:
                            break;
                    }
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

    public int GetAttackStrength()
    {
        int damage = this.Stats.BaseStrength;
        if (this.Stats.Inventory.EquippedWeapon != null)
        {
            damage += this.Stats.Inventory.EquippedWeapon.Data.Power;
        }

        return damage;
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
    public int MaxHP = 10;
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

public enum PlayerInteraction
{
    None,
    Move,
    InteractWithObject,
    Attack
}