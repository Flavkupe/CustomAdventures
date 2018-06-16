using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class Player : TileEntity, IDungeonActor
{
    public GameObject Actor => this.gameObject;

    private static Player instance;

    public int AbilityThreshold = 3;

    public override TileEntityType EntityType { get { return TileEntityType.Player; } }

    public static Player Instance { get { return instance; } private set { instance = value; } }

    private AnimatedEquipment _animatedWeapon = null;

    public List<IAbilityCard> Abilities { get { return _abilities; } }

    public PlayerStats Stats;
    public Stats BaseStats;

    public Stats CurrentStats { get { return Stats; } }

    public PlayerInventory Inventory;

    public List<StatusEffect> Effects { get; } = new List<StatusEffect>();

    private List<IAbilityCard> _abilities = new List<IAbilityCard>();

    public DungeonCardData[] EntranceCards;

    private SoundGenerator _soundGen;

    private Direction _facingDirection = Direction.Right;

    private int _fullActions;
    private int _freeMoves;

    // Sounds
    public AudioClip[] DamagedSounds;
    public AudioClip[] UnarmedHitSounds;
    public AudioClip LevelupSound;

    // Stuff like the decks and such
    public GameObject InterfaceObjects;

    // Use this for initialization
    private void Awake()
    {
        Instance = this;
        _soundGen = GetComponent<SoundGenerator>();
    }

    public void InitializeCombatTurn()
    {
        _fullActions = Stats.FullActions;
        _freeMoves = Stats.FreeMoves;
    }

    public void EquipAbility(IAbilityCard ability)
    {
        _abilities.Add(ability);
        AbilityPanel.Instance.SyncSlotsWithPlayer();
    }

    public void ForgetAbility(IAbilityCard ability)
    {
        Debug.Assert(_abilities.Contains(ability));
        _abilities.Remove(ability);
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
        // TODO: targetted _abilities
        Debug.Assert(_abilities.Contains(ability));
        ability.ActivateAbility();
    }

    public void ApplyEffect(StatusEffect effect)
    {
        effect.Apply(this);
        Effects.Add(effect);
    }

    public void DoHealing(int healing)
    {
        CurrentStats.HP += healing;
        ShowFloatyText("+" + healing, Color.green, FloatyTextSize.Small);
        BlinkColor(Color.green);
    }

    public override void DoDamage(int damage)
    {
        var defensiveItems = Inventory.GetDefensiveItems();
        defensiveItems.Shuffle(); // randomize order for mitigation order

        foreach (var item in defensiveItems)
        {
            damage -= item.DefenseValue;
            item.ItemUsed();

            if (damage <= 0)
            {
                damage = 0;
                break;
            }
        }

        if (damage > 0)
        {
            Stats.HP -= damage;
            ShowFloatyText("-" + damage, null, FloatyTextSize.Small);
            if (Stats.HP <= 0)
            {
                Die();
            }
            else
            {
                BlinkColor(Color.red);
                _soundGen.PlayRandomFrom(DamagedSounds);
            }
        }
        else
        {
            ShowFloatyText("Blocked!", Color.white, FloatyTextSize.Medium);
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
        ShowFloatyText(exp.ToString() + " XP", Color.white, FloatyTextSize.Medium);
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
        routine.Then(() => ShowFloatyText("LEVEL UP!", Color.yellow, FloatyTextSize.Large))
               .Then(() => Game.CardDraw.PerformCharacterCardDrawing(2))
               .Then(() => Game.UI.UpdateUI());
        Game.States.EnqueueIfNotState(GameState.CharacterMoving, routine);
        _soundGen.PlayClip(LevelupSound);
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
        if (Inventory.IsSlotOccupied(InventoryItemType.Weapon))
        {
            Inventory.EquippedWeapon.ItemUsed();
        }

        OnAfterPlayerAction(true);
    }

    private void OnAfterPlayerAction(bool isFullAction)
    {        
        if (Game.Dungeon.IsCombat)
        {
            if (!isFullAction && _freeMoves > 0)
            {
                _freeMoves--;
            }
            else
            {
                _fullActions--;
                _freeMoves = 0;
            }
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
        if (direction == Direction.Left || direction == Direction.Right)
        {
            FaceDirection(direction);
        }

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
            PlayAttackEffects();
            routine.Then(() => OnAfterPlayerAttack());
        }
        else if (interaction == PlayerInteraction.InteractWithObject)
        {
            routine.Then(() => OnAfterPlayerInteract());
        }
        
        Game.States.EnqueueCoroutine(routine);
    }

    private void PlayAttackEffects()
    {
        var weapon = Inventory.EquippedWeapon;
        if (weapon != null)
        {
            if (_animatedWeapon != null)
            {
                _animatedWeapon.AnimateOnce();
            }

            if (weapon.Data.HitSounds.Length > 0)
            {
                _soundGen.PlayRandomFrom(weapon.Data.HitSounds);
                return;
            }
        }

        _soundGen.PlayRandomFrom(UnarmedHitSounds);
    }

    public int GetAttackStrength()
    {
        int damage = Stats.BaseStrength;
        if (Inventory.EquippedWeapon != null)
        {
            damage += Inventory.EquippedWeapon.Data.Power;
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

    public void PlayClip(AudioClip clip)
    {
        _soundGen.PlayClip(clip);
    }

    public void PlaySounds(IEnumerable<AudioClip> clips, AudioClip defaultClip = null)
    {
        _soundGen.PlayRandomFrom(clips, defaultClip);
    }

    public DungeonCardData[] GetEntranceCards()
    {
        // TODO: other classes
        return EntranceCards;
    }

    public void DestroyItem(InventoryItem item)
    {
        Inventory.DestroyInventoryItem(item);
        if (item.ItemData.ItemType == InventoryItemType.Weapon)
        {
            item.ItemBroken();
            ShowFloatyText("Weapon broke!", Color.white, FloatyTextSize.Medium);
        }
        else if (item.DefenseValue > 0)
        {
            item.ItemBroken();
            ShowFloatyText("Armor broke!", Color.white, FloatyTextSize.Medium);
        }
    }

    public void SetAnimatedWeapon(AnimatedEquipment equipment)
    {
        if (_animatedWeapon == equipment)
        {
            return;
        }

        if (_animatedWeapon != null)
        {
            Destroy(_animatedWeapon.gameObject);
        }

        if (equipment == null)
        {
            _animatedWeapon = null;
            return;
        }

        _animatedWeapon = Instantiate(equipment);
        _animatedWeapon.transform.SetParent(this.transform);
        _animatedWeapon.transform.localPosition = new Vector3();
        _animatedWeapon.FaceDirection(_facingDirection);
    }

    private void FaceDirection(Direction direction)
    {
        GetComponent<SpriteRenderer>().flipX = direction == Direction.Left;
        _facingDirection = direction;
        if (_animatedWeapon != null)
        {
            _animatedWeapon.FaceDirection(direction);
        }
    }

    public int FreeMoves
    {
        get { return CurrentStats.FreeMoves; } set { CurrentStats.FreeMoves = value; }
    }

    public int FullActions
    {
        get { return CurrentStats.FullActions;} set { CurrentStats.FullActions = value; }
    }

    public void AfterAppliedStatusEffect(StatusEffectData effect)
    {
    }
}

[Serializable]
public class PlayerStats : Stats
{
    public int Mulligans = 2;
    public int EXP = 0;
}

public enum PlayerInteraction
{
    None,
    Move,
    InteractWithObject,
    Attack
}