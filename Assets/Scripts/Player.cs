using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class Player : TileEntity
{
    private static Player instance;

    public int AbilityThreshold = 3;

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
        AbilityPanel.Instance.SyncSlotsWithPlayer();
    }

    public void ForgetAbility(IAbilityCard ability)
    {
        Debug.Assert(this.abilities.Contains(ability));        
        this.abilities.Remove(ability);
        AbilityPanel.Instance.SyncSlotsWithPlayer();
    }

    public void UseAbility(IAbilityCard ability)
    {
        // TODO: targetted abilities
        Debug.Assert(this.abilities.Contains(ability));
        ability.ActivateAbility();        
    }

    public void ApplyEffect(StatusEffect effect)
    {
        effect.Apply(this);
        Effects.Add(effect);
    }

    public void TakeDamage(int attack)
    {
        this.Stats.HP -= attack;
        this.ShowFloatyText("-" + attack);
        if (this.Stats.HP <= 0)
        {
            this.Die();
        }        
    }

    private void Die()
    {
        // TODO
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        if (StateManager.Instance.IsPaused)
        {
            return;
        }

        if (StateManager.Instance.State == GameState.AwaitingCommand)
        {
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

        if (!drawingAbilities && this.Abilities.Count < this.AbilityThreshold && Game.Decks.AbilityDeck.CardCount > 0)
        {
            int numToDraw = this.AbilityThreshold - this.Abilities.Count;
            numToDraw = Mathf.Min(Game.Decks.AbilityDeck.CardCount, numToDraw);
            this.DrawAbilities(numToDraw);
        }
    }

    private bool drawingAbilities = false;
    private void DrawAbilities(int num)
    {
        if (num > 0)
        {
            drawingAbilities = true;
            Game.CardDraw.PerformAbilityCardDrawing(num).Finally(() => drawingAbilities = false);
        }
    }

    public void GainXP(int exp)
    {
        this.ShowFloatyText(exp.ToString() + " XP");
        this.Stats.EXP += exp;

        if ((this.Stats.EXP / 10) + 1 > this.Stats.Level)
        {
            this.LevelUp();
        }
        
    }

    public void LevelUp()
    {
        this.Stats.Level++;
        Routine routine = Routine.Create(() => Routine.WaitForSeconds(0.5f));
        routine.Then(() => this.ShowFloatyText("LEVEL UP!"))
               .Then(() => Game.CardDraw.PerformCharacterCardDrawing(2))
               .Then(() => Game.UI.UpdateUI());
        Game.States.EnqueueIfNotState(GameState.CharacterMoving, routine);
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
        DungeonManager.Instance.AfterPlayerTurn();
    }

    private IEnumerator TryPlayerMove(Direction direction)
    {
        yield return StartCoroutine(this.TryMove(direction));
        OnAfterPlayerMove();
        DungeonManager.Instance.AfterPlayerTurn();
    }

    public void PlayerMoveCommand(Direction direction)
    {
        TileGrid grid = DungeonManager.Instance.Grid;
        if (this.CanMove(direction))
        {            
            StartCoroutine(this.TryPlayerMove(direction));
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

    public int Mulligans = 2;

    public int EXP = 0;

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