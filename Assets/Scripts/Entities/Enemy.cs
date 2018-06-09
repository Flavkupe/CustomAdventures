using System.Collections;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SoundGenerator))]
public class Enemy : TileEntity, IDungeonActor
{
    public EnemyCardData Data { get; set; }

    public override TileEntityType EntityType { get { return TileEntityType.Enemy; } }

    private int HP;

    private SoundGenerator _soundGen;

    public int FullActions { get; set; } 
    public int FreeMoves { get; set; }

    public IEnumerator ProcessCharacterTurn()
    {
        FullActions = Data.FullActions;
        FreeMoves = Data.FreeMoves;

        while (FullActions > 0 || FreeMoves > 0)
        {
            var performedMove = false;

            // Full actions
            if (FullActions > 0)
            {                
                if (Game.Dungeon.Grid.GetNeighbors(XCoord, YCoord).Where(t => t != null && t.GetTileEntity() != null)
                                                                            .Select(a => a.GetTileEntity()).Any(b => b == Game.Player))
                {
                    FullActions--;
                    performedMove = true;
                    yield return AttackPlayer();
                }
            }

            // Move actions
            if (!performedMove && (FullActions > 0 || FreeMoves > 0))
            {
                // Basic move
                Direction? dir = null;
                if (Game.Player.XCoord > XCoord && CanMove(Direction.Right))
                {
                    dir = Direction.Right;
                }
                else if (Game.Player.XCoord < XCoord && CanMove(Direction.Left))
                {
                    dir = Direction.Left;
                }
                else if (Game.Player.YCoord > YCoord && CanMove(Direction.Up))
                {
                    dir = Direction.Up;
                }
                else if (Game.Player.YCoord < YCoord && CanMove(Direction.Down))
                {
                    dir = Direction.Down;
                }

                if (dir != null)
                {
                    if (FreeMoves > 0) FreeMoves--;
                    else FullActions--;
                    performedMove = true;
                    yield return TryMove(dir.Value);
                }
            }

            if (!performedMove)
            {
                // No moves; skip turn
                FullActions = 0;
                FreeMoves = 0;
                yield break;
            }
        }
    }

    public IEnumerator AttackPlayer()
    {
        yield return TwitchTowards(Game.Player.transform.position);
        Game.Player.DoDamage(Data.Attack);
    }

    public override void DoDamage(int damage)
    {
        if (HP > 0)
        {
            HP -= damage;
            ShowFloatyText("-" + damage.ToString(), null, FloatyTextSize.Small);
            if (HP <= 0)
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

    protected override void OnClicked()
    {
        Game.UI.UpdateEntityPanel(this);
        base.OnClicked();
    }

    [UsedImplicitly]
    private void Start ()
    {
        HP = Data.MaxHP;
        GetComponent<SpriteRenderer>().sprite = Data.Sprite;
        GetComponent<SpriteRenderer>().sortingLayerName = "Entities";
        _soundGen = GetComponent<SoundGenerator>();
        GetComponent<BoxCollider2D>().size = new Vector3(1.0f, 1.0f);
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
}
