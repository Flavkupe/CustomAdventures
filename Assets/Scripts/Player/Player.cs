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

    public DungeonCardData[] EntranceCards;

    private int _fullActions;
    private int _freeMoves;

    // Stuff like the decks and such
    public GameObject InterfaceObjects;

    // Use this for initialization
    private void Awake()
    {
        Instance = this;        
    }

    public void InitializeCombatTurn()
    {
        _fullActions = Stats.FullActions;
        _freeMoves = Stats.FreeMoves;
    }

    public void EquipAbility(IAbilityCard ability)
    {
        abilities.Add(ability);
        AbilityPanel.Instance.SyncSlotsWithPlayer();
    }

    public void ForgetAbility(IAbilityCard ability)
    {
        Debug.Assert(abilities.Contains(ability));
        abilities.Remove(ability);
        ability.DestroyCard();
        AbilityPanel.Instance.SyncSlotsWithPlayer();
    }

    public void AfterAbilityUsed(IAbilityCard ability)
    {
        ProcessEffects(EffectDurationType.Attacks);
        OnAfterPlayerAction(true);
    }

    public void UseAbility(IAbilityCard ability)
    {
        // TODO: targetted abilities
        Debug.Assert(abilities.Contains(ability));
        ability.ActivateAbility();
    }

    public void ApplyEffect(StatusEffect effect)
    {
        effect.Apply(this);
        Effects.Add(effect);
    }

    public override void DoDamage(int damage)
    {
        Stats.HP -= damage;
        ShowFloatyText("-" + damage);
        if (Stats.HP <= 0)
        {
            Die();
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

    private void Update ()
    {
        if (Game.States.IsPaused || Game.UI.IsMenuActive)
        {
            return;
        }

        if (Game.States.State == GameState.AwaitingCommand)
        {
            if (Input.GetKey(KeyCode.W))
            {
                PlayerMoveCommand(Direction.Up);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                PlayerMoveCommand(Direction.Down);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                PlayerMoveCommand(Direction.Left);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                PlayerMoveCommand(Direction.Right);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                PlayerSkipTurn();
            }
        }

        if (!drawingAbilities && Abilities.Count < AbilityThreshold && Game.Decks.AbilityDeck.CardCount > 0)
        {
            int numToDraw = AbilityThreshold - Abilities.Count;
            numToDraw = Mathf.Min(Game.Decks.AbilityDeck.CardCount, numToDraw);
            DrawAbilities(numToDraw);
        }
    }

    private void PlayerSkipTurn()
    {
        _freeMoves = 0;
        _fullActions = 0;
        OnAfterPlayerMove();
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
        ShowFloatyText(exp.ToString() + " XP");
        Stats.EXP += exp;

        if ((Stats.EXP / 10) + 1 > Stats.Level)
        {
            LevelUp();
        }
        
    }

    public void LevelUp()
    {
        Stats.Level++;
        Routine routine = Routine.Create(() => Routine.WaitForSeconds(0.5f));
        routine.Then(() => ShowFloatyText("LEVEL UP!"))
               .Then(() => Game.CardDraw.PerformCharacterCardDrawing(2))
               .Then(() => Game.UI.UpdateUI());
        Game.States.EnqueueIfNotState(GameState.CharacterMoving, routine);
    }

    private void ProcessEffects(EffectDurationType actionTaken)
    {
        foreach (StatusEffect effect in Effects.ToList())
        {
            if (effect.DurationType == actionTaken)
            {
                effect.Duration--;
                if (effect.Duration <= 0)
                {
                    EffectExpire(effect);
                }
            }
        }
    }

    private void OnAfterPlayerInteract()
    {
        ProcessEffects(EffectDurationType.Steps);
        OnAfterPlayerAction(true);
    }

    private void OnAfterPlayerMove()
    {        
        ProcessEffects(EffectDurationType.Steps);
        OnAfterPlayerAction(false);
    }

    private void OnAfterPlayerAttack()
    {
        ProcessEffects(EffectDurationType.Attacks);
        if (Stats.Inventory.IsSlotOccupied(InventoryItemType.Weapon))
        {
            Stats.Inventory.EquippedWeapon.ItemUsed();
        }

        OnAfterPlayerAction(true);
    }

    private void OnAfterPlayerAction(bool isFullAction)
    {        
        if (Game.Dungeon.IsCombat)
        {
            if (!isFullAction && _freeMoves > 0) _freeMoves--;
            else _fullActions--;
        }

        Game.Dungeon.AfterPlayerTurn();        
    }

    private IEnumerator TryPlayerMove(Direction direction)
    {
        if (Game.Dungeon.IsCombat)
        {
            var adjacentEnemies = Game.Dungeon.GetEntitiesNearPlayer(TileRangeType.Sides, 1, TileEntityType.Enemy);
            yield return ActivateOpportunityAttacks(adjacentEnemies.OfType<Enemy>());
        }

        yield return TryMove(direction);        
        OnAfterPlayerMove();        
    }

    private IEnumerator ActivateOpportunityAttacks(IEnumerable<Enemy> enemies)
    {
        foreach (var enemy in enemies)
        {
            yield return enemy.AttackPlayer();
        }
    }

    public bool PlayerCanAct { get { return !Game.Dungeon.IsCombat || _fullActions > 0; } }
    public bool PlayerCanMove { get { return PlayerCanAct || _freeMoves > 0; } }

    public void PlayerMoveCommand(Direction direction)
    {
        TileGrid grid = Game.Dungeon.Grid;
        if (PlayerCanMove && CanMove(direction))
        {
            // Set state to ensure we don't queue multiple moves
            Game.States.SetState(GameState.CharacterMoving);
            Game.States.EnqueueCoroutine(() => TryPlayerMove(direction));
        }
        else if (PlayerCanAct)
        {
            TileEntity obj = grid.GetAdjacentObject(XCoord, YCoord, direction);
            if (obj != null)
            {
                if (obj.PlayerCanInteractWith())
                {
                    Game.States.SetState(GameState.CharacterActing);
                    InteractWith(obj);
                }
            }
            else
            {
                // Boundry
            }
        }
        
        Game.UI.UpdateUI();
    }

    private void InteractWith(TileEntity obj)
    {
        var interaction = obj.GetPlayerInteraction(this);
        var routine = Routine.Create(() => obj.PlayerInteractWith());
        
        if (interaction == PlayerInteraction.Attack)
        {
            routine.Then(() => OnAfterPlayerAttack());
        }
        else if (interaction == PlayerInteraction.InteractWithObject)
        {
            routine.Then(() => OnAfterPlayerInteract());
        }
        
        Game.States.EnqueueCoroutine(routine);
    }

    public int GetAttackStrength()
    {
        int damage = Stats.BaseStrength;
        if (Stats.Inventory.EquippedWeapon != null)
        {
            damage += Stats.Inventory.EquippedWeapon.Data.Power;
        }

        return damage;
    }

    public override IEnumerator TwitchTowards(Direction direction, float speed = 5.0f)
    {
        Camera.main.transform.SetParent(null);
        InterfaceObjects.transform.SetParent(null);
        yield return base.TwitchTowards(direction, speed);
        InterfaceObjects.transform.SetParent(transform);
        Camera.main.transform.SetParent(transform);
        
    }

    public DungeonCardData[] GetEntranceCards()
    {
        // TODO: other classes
        return EntranceCards;
    }

    public void DestroyItem(InventoryItem item)
    {
        Stats.Inventory.DestroyInventoryItem(item);
        if (item.ItemData.ItemType == InventoryItemType.Weapon)
        {
            ShowFloatyText("Weapon broke!", Color.white, 5);
        }
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

    public int FullActions = 1;
    public int FreeMoves = 1;

    public int EXP = 0;

    public PlayerInventory Inventory;
}

public enum PlayerInteraction
{
    None,
    Move,
    InteractWithObject,
    Attack
}