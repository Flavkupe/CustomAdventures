﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;
using Assets.Scripts.State;
using JetBrains.Annotations;
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
    private readonly PlayerStats _stats = new PlayerStats();

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

    public event EventHandler<IAbilityCard> AbilityCardUsed;

    public PlayerStateController StateController { get; } = new PlayerStateController();

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

        Game.Tokens.SetToken("PlayerHP", () => this.CurrentStats.HP.ToString());
        Game.Tokens.SetToken("PlayerMaxHP", () => this.BaseStats.HP.ToString());
        Game.Tokens.SetToken("Mulligans", () => this.GetPlayerStats().Mulligans.Value.ToString());
    }

    public void InitializePlayerTurn()
    {
        var baseStats = GetModifiedStats(true);
        CurrentStats.FullActions.Value = baseStats.FullActions.Value;
        CurrentStats.FreeMoves.Value = baseStats.FreeMoves.Value;
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
        // TODO: put in player state... somehow?
        ProcessEffects(EffectActivatorType.Attacks);
        if (AbilityCardUsed != null)
        {
            AbilityCardUsed.Invoke(this, ability);
        }
    }

    public void UseAbility(IAbilityCard ability)
    {
        Debug.Assert(_abilities.Contains(ability));
        ability.ActivateAbility(Game.Dungeon.GetGameContext());
    }

    public override void DoHealing(int healing)
    {
        CurrentStats.HP.Value += healing;
        ShowFloatyText("+" + healing, Color.green, FloatyTextSize.Small);
        BlinkColor(Color.green);
    }

    public override void AddStatusEffect(PersistentStatusEffect effect)
    {
        base.AddStatusEffect(effect);
        var persistentEffect = effect as PersistentStatusEffect;
        if (persistentEffect != null && Game.UI?.EffectPanel)
        {
            Game.UI.EffectPanel.CreateIcon(persistentEffect);
        }
    }

    protected override void OnEffectExpired(PersistentStatusEffect effect)
    {
        if (Game.UI?.EffectPanel)
        {
            Game.UI.EffectPanel.ExpireEffect(effect);
        }
    }

    public override void DoDamage(int damage)
    {
        var defensiveItems = Inventory.GetDefensiveItems();
        defensiveItems.Shuffle(); // randomize order for mitigation order

        foreach (var item in defensiveItems)
        {
            damage -= item.DefenseValue;
            item.ItemDurabilityExpended();

            if (damage <= 0)
            {
                damage = 0;
                break;
            }
        }

        if (damage > 0)
        {
            CurrentStats.HP.Value -= damage;
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

    private void Update()
    {
        StateController.Update(Game.Dungeon.GetGameContext());

        if (!_drawingAbility && Abilities.Count < AbilityThreshold && Game.Decks.AbilityDeck.CardCount > 0)
        {
            DrawAbility();
        }
    }

    private void DrawAbility()
    {
        _drawingAbility = true;
        AbilityCardNeeded?.Invoke(this, this);
    }

    public void GainXP(int exp)
    {
        ShowFloatyText(exp + " XP", Color.white, FloatyTextSize.Medium);
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

    /// <summary>
    /// Updates status effects, buffs and enchantments affected by specific actions being
    /// taken, such as walking or attacking
    /// </summary>
    public void ProcessEffects(EffectActivatorType actionTaken)
    {
        foreach (PersistentStatusEffect effect in Effects.ToList())
        {
            effect.ActionTaken(this, actionTaken);
        }
    }

    

    public bool PlayerHasActions => CurrentStats.FullActions > 0;
    public bool PlayerHasMoves => PlayerHasActions || CurrentStats.FreeMoves > 0;

    public void PlayAttackEffects()
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

    public IEnumerator TwitchTowards(TileEntity other, float speed = 5.0f)
    {
        var playerDirection = transform.position.GetRelativeDirection(other.transform.position);
        yield return TwitchTowards(playerDirection, speed);
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
            item.ItemBroke();
            ShowFloatyText("Weapon broke!", Color.white, FloatyTextSize.Medium);
        }
        else if (item.DefenseValue > 0)
        {
            item.ItemBroke();
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

    public void FaceDirection(Direction direction)
    {
        GetComponent<SpriteRenderer>().flipX = direction == Direction.Left;
        _facingDirection = direction;
        if (_animatedWeapon != null)
        {
            _animatedWeapon.FaceDirection(direction);
        }
    }

    private void Start()
    {
        StateController.Start();
    }

    /// <summary>
    /// Call this method exactly once, after the dungeon starts. Use this to register
    /// all startup events for the dungeon.
    /// </summary>
    public void DungeonStarted(Dungeon dungeon)
    {
        // TODO: revamp with FSM!
        if (dungeon.IsCombat)
        {
            InitializePlayerTurn();
        }

        dungeon.EnemyListPopulated += HandleCombatShouldStart;
        dungeon.EnemyListChanged += HandleEnemyListChanged;
    }

    private void HandleEnemyListChanged(object sender, List<Enemy> e)
    {
        Game.Dungeon.BroadcastEvent(PlayerEventType.CombatStateChanged);
    }

    private void HandleCombatShouldStart(object sender, EventArgs e)
    {
        Game.Dungeon.BroadcastEvent(PlayerEventType.InitializeCombat);
    }
}

[Serializable]
public class PlayerStats : Stats
{
    public PlayerStats()
    {
        Mulligans.Value = 2;
        Mulligans.PropertyChanged += OnPropertyChanged;
    }

    public readonly IntObservable Mulligans = new IntObservable();

    public int EXP = 0;

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (Game.World != null)
        {
            Game.World.SimpleEventHappened(SimpleWorldEvent.PlayerStatsChange);
        }
    }
}



public enum PlayerInteraction
{
    None,
    Move,
    InteractWithObject,
    Attack
}

