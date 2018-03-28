using System.Collections;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Enemy : TileEntity, IDungeonActor
{
    public EnemyCardData Data { get; set; }

    public override TileEntityType EntityType { get { return TileEntityType.Enemy; } }

    private int HP;

    public IEnumerator ProcessCharacterTurn()
    {
        if (Game.Dungeon.Grid.GetNeighbors(XCoord, YCoord).Where(t => t != null && t.GetTileEntity() != null)
                                                                    .Select(a => a.GetTileEntity()).Any(b => b == Game.Player))
        {
            yield return AttackPlayer();
            yield break;
        }

        // Basic move
        if (Game.Player.XCoord > XCoord)
        {
            yield return TryMove(Direction.Right);
        }
        else if (Game.Player.XCoord < XCoord)
        {
            yield return TryMove(Direction.Left);
        }
        else if (Game.Player.YCoord > YCoord)
        {
            yield return TryMove(Direction.Up);
        }
        else if (Game.Player.YCoord < YCoord)
        {
            yield return TryMove(Direction.Down);
        }
    }

    private IEnumerator AttackPlayer()
    {
        yield return TwitchTowards(Game.Player.transform.position);
        Game.Player.TakeDamage(Data.Attack);
    }

    public override void DoDamage(int damage)
    {
        TakeDamage(damage);
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
    }

    public void TakeDamage(int damage)
    {
        if (HP > 0)
        {
            HP -= damage;
            ShowFloatyText(damage.ToString());
            if (HP <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Game.Dungeon.RemoveEnemy(this);
        Destroy(gameObject, 0.5f);
        Game.Player.GainXP(Data.EXP);
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
        TakeDamage(damage);        
    }
}
