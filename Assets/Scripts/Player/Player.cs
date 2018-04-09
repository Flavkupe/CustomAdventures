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

    // Stuff like the decks and such
    public GameObject InterfaceObjects;

    // Use this for initialization
    private void Awake()
    {
        Instance = this;
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

    public void TakeDamage(int attack)
    {
        Stats.HP -= attack;
        ShowFloatyText("-" + attack);
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

    private void Start()
    {
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
        }

        if (!drawingAbilities && Abilities.Count < AbilityThreshold && Game.Decks.AbilityDeck.CardCount > 0)
        {
            int numToDraw = AbilityThreshold - Abilities.Count;
            numToDraw = Mathf.Min(Game.Decks.AbilityDeck.CardCount, numToDraw);
            DrawAbilities(numToDraw);
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
        Game.Dungeon.AfterPlayerTurn();
    }

    private void OnAfterPlayerMove()
    {
        ProcessEffects(EffectDurationType.Steps);
    }

    private void OnAfterPlayerAttack()
    {
        ProcessEffects(EffectDurationType.Attacks);
        Game.Dungeon.AfterPlayerTurn();
    }

    private IEnumerator TryPlayerMove(Direction direction)
    {
        yield return TryMove(direction);
        OnAfterPlayerMove();
        Game.Dungeon.AfterPlayerTurn();
    }

    public void PlayerMoveCommand(Direction direction)
    {
        TileGrid grid = Game.Dungeon.Grid;
        if (CanMove(direction))
        {            
            Game.States.EnqueueCoroutine(() => TryPlayerMove(direction));
        }
        else
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

public enum PlayerInteraction
{
    None,
    Move,
    InteractWithObject,
    Attack
}