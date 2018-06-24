using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SoundGenerator))]
[RequireComponent(typeof(BehaviorController))]
public class Enemy : TileAI
{
    public EnemyCardData Data { get; set; }

    public override TileEntityType EntityType => TileEntityType.Enemy;

    public override BehaviorList Behavior => Data.Behavior;

    private Stats _stats = new Stats();

    public override Stats CurrentStats => _stats;

    private SoundGenerator _soundGen;
    private BehaviorController _behavior;

    public override IEnumerator ProcessCharacterTurn()
    {
        // Initialize stats for start of turn
        CurrentStats.FreeMoves = Data.BaseStats.FreeMoves;
        CurrentStats.FullActions = Data.BaseStats.FullActions;

        // Do all strategies from behavior list
        yield return _behavior.DoStrategy(new GameContext {Dungeon = Game.Dungeon, Player = Game.Player});
    }

    public IEnumerator AttackPlayer()
    {
        yield return TwitchTowards(Game.Player.transform.position);
        Game.Player.DoDamage(Data.BaseStats.BaseStrength);
    }

    public override void DoDamage(int damage)
    {
        if (_stats.HP > 0)
        {
            _stats.HP -= damage;
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
    }

    protected override void Init()
    {
        base.Init();
    }

    private void Die()
    {
        if (Data.DeathEffect != null)
        {
            var effect = Game.Effects.GenerateAnimationEffect(Data.DeathEffect);
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

    public override IEnumerator PlayerInteractWith()
    {
        var playerDirection = Game.Player.transform.position.GetRelativeDirection(transform.position);
        yield return Game.Player.TwitchTowards(playerDirection);
        int damage = Game.Player.GetAttackStrength();
        DoDamage(damage);
    }

    public override void AfterAppliedStatusEffect(StatusEffectData effect)
    {
        // TODO
    }

    public override void SpawnOnGrid(DungeonManager dungeon, GridTile tile)
    {
        base.SpawnOnGrid(dungeon, tile);
        dungeon.RegisterEnemy(this);
    }
}
