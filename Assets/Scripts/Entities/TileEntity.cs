using System;
using System.Collections;

public abstract class TileEntity : MonoBehaviourEx, IObjectOnTile
{
    public float TileSlideSpeed = 10.0f;

    public int XCoord { get; set; }
    public int YCoord { get; set; }

    public bool Selected { get; set; }

    public abstract TileEntityType EntityType { get; }

    public void ShowFloatyText(string text)
    {
        FloatyText damageText = Instantiate(TextManager.Instance.DamageTextTemplate);
        damageText.Init(transform.position, text);
    }

    protected virtual void OnClicked()
    {
    }

    private void OnMouseDown()
    {
        OnClicked();
    }

    public virtual bool PlayerCanInteractWith()
    {
        return false;
    }

    public virtual void DoDamage(int damage)
    {
    }

    public virtual PlayerInteraction GetPlayerInteraction(Player player)
    {
        return PlayerInteraction.None;
    }

    public virtual IEnumerator PlayerInteractWith()
    {
        yield return null;
    }

    public bool CanMove(Direction direction)
    {
        TileGrid grid = Game.Dungeon.Grid;
        return grid.CanOccupyAdjacent(XCoord, YCoord, direction);
    }

    public IEnumerator TryMove(Direction direction)
    {
        if (!CanMove(direction))
        {
            yield break;
        }

        Game.States.SetState(GameState.CharacterMoving);
        TileGrid grid = Game.Dungeon.Grid;
        grid.MoveTo(XCoord, YCoord, direction, this);
        GridTile newTile = grid.GetTile(XCoord, YCoord);
        yield return transform.MoveToSpotCoroutine(newTile.transform.position, TileSlideSpeed, false);
        Game.States.RevertState();
    }

    public virtual void RemoveFromGrid()
    {
        TileGrid grid = Game.Dungeon.Grid;
        grid.ClearTileEntity(this.XCoord, this.YCoord);
    }

    private void OnMouseUp()
    {
        if (Game.States.State == GameState.AwaitingSelection)
        {
            Selected = !Selected;
            Game.Dungeon.AfterToggledSelection(this);
        }
    }
}

[Flags]
public enum TileEntityType
{
    Player = 1,
    Enemy = 2,
    Environment = 4,
    Item = 8,
}
