using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileEntity : MonoBehaviourEx, IObjectOnTile
{
    public float TileSlideSpeed = 10.0f;

    public int XCoord { get; set; }
    public int YCoord { get; set; }

    public bool Selected { get; set; }

    public abstract TileEntityType EntityType { get; }

    Queue<FloatyText> floatyTextQueue = new Queue<FloatyText>();

    // TODO: move this to a new component class
    private bool floatyTextShowing = false;
    public void ShowFloatyText(string text, Color? color = null, float? size = null)
    {
        var damageText = Instantiate(TextManager.Instance.DamageTextTemplate);
        damageText.Init(transform.position, text, 0.5f, 1.0f, !floatyTextShowing);
        damageText.TextFinished += DamageText_TextFinished;
        if (floatyTextShowing)
        {
            floatyTextQueue.Enqueue(damageText);
        }
        else
        {
            floatyTextShowing = true;
        }

        if (color != null)
        {
            damageText.SetColor(color.Value);
        }

        if (size != null)
        {
            damageText.SetSize(size.Value);
        }
    }

    private void DamageText_TextFinished(object sender, EventArgs e)
    {
        if (floatyTextQueue.Count == 0)
        {
            floatyTextShowing = false;
        }
        else
        {
            var text = floatyTextQueue.Dequeue();
            floatyTextShowing = true;
            text.TextFinished += DamageText_TextFinished;
            text.transform.position = this.transform.position;
            text.Activate();
        }
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

        yield return AfterMove(newTile);
        Game.States.RevertState();
    }

    public IEnumerator AfterMove(GridTile newTile)
    {
        foreach (var passable in newTile.GetPassableTileEntities())
        {
            yield return passable.ProcessEntitySteppedOver(this);
        }
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
    Trap = 16,
}
