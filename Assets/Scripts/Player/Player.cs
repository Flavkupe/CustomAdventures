using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class Player : TileActor
{
    public GameObject Actor => this.gameObject;

    public int AbilityThreshold = 3;

    private bool _drawingAbility = false;

    public override TileEntityType EntityType => TileEntityType.Player;

    public static Player Instance { get; private set; }

    private AnimatedEquipment _animatedWeapon;

    public List<IAbilityCard> Abilities => _abilities;

    [SerializeField]
    private PlayerStats _stats = new PlayerStats();

    private PlayerStats _basePlayerStats { get; } = new PlayerStats();

    public PlayerStats GetPlayerStats() { return _stats; }

    public override Stats BaseStats => _basePlayerStats;

    public override Stats CurrentStats => _stats;

    public PlayerInventory Inventory;

    private readonly List<IAbilityCard> _abilities = new List<IAbilityCard>();

    public DungeonCardData[] EntranceCards;

    private SoundGenerator _soundGen;

    private Direction _facingDirection = Direction.Right;

    public event EventHandler<Player> AbilityCardNeeded;

    public event EventHandler<Player> LevelupEvent;

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

    private void InitializePlayerTurn()
    {
        var baseStats = GetModifiedStats(true);
        CurrentStats.FullActions = baseStats.FullActions;
        CurrentStats.FreeMoves = baseStats.FreeMoves;

        // TODO: revamp!
        Game.States.SetState(GameState.AwaitingCommand);
    }

    private void OnInitializeCombatTurn(object source, EventArgs args)
    {
        InitializePlayerTurn();
    }

    public void EquipDrawnAbilityCard(IAbilityCard ability)
    {
        _abilities.Add(ability);
        AbilityPanel.Instance.SyncSlotsWithPlayer(this);
        _drawingAbility = false;
    }

    public void ForgetAbility(IAbilityCard ability)
    {
        Debug.Assert(_abilities.Contains(ability));
        _abilities.Remove(ability);
        ability.DestroyCard();
        AbilityPanel.Instance.SyncSlotsWithPlayer(this);
    }

    public void AfterAbilityUsed(IAbilityCard ability)
    {
        ProcessEffects(EffectActivatorType.Attacks);
        OnAfterPlayerAction(true);
    }

    public void UseAbility(IAbilityCard ability)
    {
        Debug.Assert(_abilities.Contains(ability));
        ability.ActivateAbility();
    }

    public override void DoHealing(int healing)
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
            CurrentStats.HP -= damage;
            ShowFloatyText("-" + damage, null, FloatyTextSize.Small);
            if (CurrentStats.HP <= 0)
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

    private void Update ()
    {
        if (Game.Dungeon.IsGamePaused || Game.States.AreMenusOpen)
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

        if (!_drawingAbility && Abilities.Count < AbilityThreshold && Game.Decks.AbilityDeck.CardCount > 0)
        {
            DrawAbility();
        }
    }

    private void PlayerSkipTurn()
    {
        CurrentStats.FreeMoves = 0;
        CurrentStats.FullActions = 0;
        OnAfterPlayerMove();
    }

    private void DrawAbility()
    {
        _drawingAbility = true;
        AbilityCardNeeded?.Invoke(this, this);
    }

    public void GainXP(int exp)
    {
        ShowFloatyText(exp.ToString() + " XP", Color.white, FloatyTextSize.Medium);
        _stats.EXP += exp;

        if ((_stats.EXP / 10) + 1 > CurrentStats.Level)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        CurrentStats.Level++;

        LevelupEvent?.Invoke(this, this);

        _soundGen.PlayClip(LevelupSound);
        Routine routine = Routine.Create(() => Routine.WaitForSeconds(0.5f));
        routine.Then(() => ShowFloatyText("LEVEL UP!", Color.yellow, FloatyTextSize.Large))
               .Then(() => Game.UI.UpdateEntityPanels());
        StartCoroutine(routine);
    }

    private void ProcessEffects(EffectActivatorType actionTaken)
    {
        foreach (PersistentStatusEffect effect in Effects.ToList())
        {
            effect.ActionTaken(this, actionTaken);
        }
    }

    private void OnAfterPlayerInteract()
    {
        ProcessEffects(EffectActivatorType.Steps);
        OnAfterPlayerAction(true);
    }

    private void OnAfterPlayerMove()
    {
        ProcessEffects(EffectActivatorType.Steps);
        OnAfterPlayerAction(false);
    }

    private void OnAfterPlayerAttack()
    {
        ProcessEffects(EffectActivatorType.Attacks);
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
            if (!isFullAction && CurrentStats.FreeMoves > 0)
            {
                CurrentStats.FreeMoves--;
            }
            else
            {
                CurrentStats.FullActions--;
                CurrentStats.FreeMoves = 0;
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

    public bool PlayerCanAct => Game.States.CanPlayerAct && !Game.Dungeon.IsCombat || CurrentStats.FullActions > 0;
    public bool PlayerCanMove => PlayerCanAct || CurrentStats.FreeMoves > 0;

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
        
        Game.UI.UpdateEntityPanels();
    }

    private void InteractWith(TileEntity obj)
    {
        var interaction = obj.GetPlayerInteraction(this);
        var routine = Routine.Create(obj.PlayerInteractWith, this);
        
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
        var stats = GetModifiedStats();
        int damage = stats.Strength;
        if (Inventory.EquippedWeapon != null)
        {
            damage += Inventory.EquippedWeapon.Data.Power;
        }

        return damage;
    }

    public override IEnumerator TwitchTowards(Direction direction, float speed = 5.0f)
    {
        // Unparent Camera for twitch so that camera doesn't twitch too
        InterfaceObjects.transform.SetParent(null);
        yield return base.TwitchTowards(direction, speed);
        InterfaceObjects.transform.SetParent(transform);
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
        return EntranceCards;
    }

    public void DestroyItem(InventoryItem item)
    {
        Inventory.ClearInventoryItem(item);
        if (item.ItemData.ItemType == InventoryItemType.Weapon)
        {
            item.PlayItemBrokenSound();
            ShowFloatyText("Weapon broke!", Color.white, FloatyTextSize.Medium);
        }
        else if (item.DefenseValue > 0)
        {
            item.PlayItemBrokenSound();
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

    /// <summary>
    /// Call this method exactly once, after the dungeon starts. Use this to register
    /// all startup events for the dungeon.
    /// </summary>
    public void DungeonStarted(Dungeon dungeon)
    {
        dungeon.EnemyListPopulated += OnInitializeCombatTurn;
        dungeon.AllEnemyTurnsComplete += OnInitializeCombatTurn;

        // TODO: revamp with FSM!
        if (dungeon.IsCombat)
        {
            this.InitializePlayerTurn();
        }

        Game.States.SetState(GameState.AwaitingCommand);
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