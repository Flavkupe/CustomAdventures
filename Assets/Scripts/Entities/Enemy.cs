using System.Collections;
using UnityEngine;
using JetBrains.Annotations;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SoundGenerator))]
[RequireComponent(typeof(BehaviorController))]
public class Enemy : TileAI
{
    [TemplatePrefab]
    public static Enemy Template;

    public EnemyCardData Data { get; set; }

    public override TileEntityType EntityType => TileEntityType.Enemy;

    public override BehaviorList Behavior => Data.Behavior;

    private Stats _stats = new Stats();

    public override Stats CurrentStats => _stats;
    public override Stats BaseStats => Data.BaseStats;

    private SoundGenerator _soundGen;
    private BehaviorController _behavior;
    private HealthHearts _hearts;

    public override IEnumerator ProcessCharacterTurn()
    {
        // Initialize stats for start of turn
        CurrentStats.FreeMoves.Value = BaseStats.FreeMoves.Value;
        CurrentStats.FullActions.Value = BaseStats.FullActions.Value;

        Trace.Info(TraceType.Behavior, $"Processing {Data.Name} turn. Moves: {CurrentStats.FreeMoves} | Actions: {CurrentStats.FullActions}");

        // Do all strategies from behavior list
        yield return _behavior.DoStrategy(new GameContext {Dungeon = Game.Dungeon, Player = Game.Player});
    }

    public IEnumerator AttackPlayer()
    {
        yield return TwitchTowards(Game.Player.transform.position);
        Game.Player.DoDamage(Data.BaseStats.Strength);
    }

    public override void DoDamage(int damage)
    {
        if (_stats.HP > 0)
        {
            _stats.HP.Value -= damage;
            _hearts.UpdateHearts(_stats.HP.Value);
            ShowFloatyText("-" + damage.ToString(), null, FloatyTextSize.Small);
            if (_stats.HP <= 0)
            {
                _soundGen.PlayRandomFrom(Data.DeathSounds);
                Die();
            }
            else
            {
                BlinkColor(Color.red);
                _soundGen.PlayRandomFrom(Data.DamagedSounds);
            }
        }
    }

    public override void DoHealing(int healing)
    {
        // TODO
    }

    protected override void OnClicked()
    {
        Game.UI.UpdateEntityPanel(this);
        base.OnClicked();
    }

    [UsedImplicitly]
    private void Start ()
    {
        Init();
        _stats = Data.BaseStats.Clone();
        GetComponent<SpriteRenderer>().sprite = Data.Sprite;
        GetComponent<SpriteRenderer>().sortingLayerName = "Entities";
        _soundGen = GetComponent<SoundGenerator>();
        GetComponent<BoxCollider2D>().size = new Vector3(1.0f, 1.0f);
        _behavior = GetComponent<BehaviorController>();

        _hearts = Instantiate(Game.Dungeon.Templates.EntityParts.HealthHearts);
        _hearts.transform.SetParent(transform);
        _hearts.transform.localPosition = new Vector3(-0.25f, 0.5f);
        _hearts.UpdateHearts(_stats.HP.Value);
    }

    protected override void Init()
    {
        base.Init();
    }

    private void Die()
    {
        HideThoughtBubble();

        if (Data.DeathEffect != null)
        {
            var effect = Data.DeathEffect.CreateEffect();
            effect.transform.position = this.transform.position;
            effect.Execute();
        }

        if (Data.LeaveOnDeath.Length > 0)
        {
            var deathSprite = Data.LeaveOnDeath.GetRandom();
            var remains = InstantiateOfType<DecorativeTileEntity>(this.name + "_remains");
            remains.SetSprite(deathSprite);
            Game.Dungeon.Grid.PutPassableEntity(this.XCoord, this.YCoord, remains, true);
        }

        Game.Dungeon.RemoveEnemy(this);
        Game.Player.GainXP(Data.EXP);
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 1.0f);
    }

    public override bool PlayerCanInteractWith()
    {
        return true;
    }

    public override PlayerInteraction GetPlayerInteraction(Player player)
    {
        return PlayerInteraction.Attack;
    }

    public override IEnumerator PlayerInteractWith(Player player)
    {
        int damage = Game.Player.GetAttackStrength();
        DoDamage(damage);
        yield return null;
    }

    public override void AfterAppliedStatusEffect(StatusEffectData effect)
    {
        // TODO
    }

    public override void SpawnOnGrid(Dungeon dungeon, GridTile tile)
    {
        base.SpawnOnGrid(dungeon, tile);
        dungeon.RegisterEnemy(this);
    }
}
