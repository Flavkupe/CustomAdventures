using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(FloatyTextGenerator))]
public abstract class TileEntity : MonoBehaviourEx, IObjectOnTile
{
    public float TileSlideSpeed = 10.0f;

    public int XCoord { get; set; }
    public int YCoord { get; set; }

    public bool Selected { get; set; }

    public abstract TileEntityType EntityType { get; }

    private bool _blinking = false;

    public void ShowFloatyText(string text, Color? color = null, FloatyTextSize? size = null)
    {
        var generator = GetComponent<FloatyTextGenerator>();
        generator.ShowFloatyText(text, color, size);
    }

    public void ShowFloatyText(FloatyTextOptions options)
    {
        var generator = GetComponent<FloatyTextGenerator>();
        generator.ShowFloatyText(options);
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

    public virtual IEnumerator PlayerInteractWith(Player player)
    {
        yield return null;
    }

    public virtual void BlinkColor(Color color, float blinkSpeed = 20.0f)
    {
        if (!_blinking)
        {
            StartCoroutine(BlinkColorCoroutine(color, blinkSpeed));
        }
    }

    private IEnumerator BlinkColorCoroutine(Color targetColor, float blinkSpeed)
    {
        if (_blinking)
        {
            yield break;
        }

        _blinking = true;
        var sprite = GetComponent<SpriteRenderer>();
        var startingColor = sprite.color;
        var t = 0.0f;
        while (t < 1.0f)
        {
            t += blinkSpeed * Time.deltaTime;
            sprite.color = Color.Lerp(startingColor, targetColor, t);
            yield return null;
        }

        while (t > 0.0f)
        {
            t -= blinkSpeed * Time.deltaTime;
            sprite.color = Color.Lerp(startingColor, targetColor, t);
            yield return null;
        }

        _blinking = false;
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

        TileGrid grid = Game.Dungeon.Grid;
        grid.MoveTo(XCoord, YCoord, direction, this);
        GridTile newTile = grid.GetTile(XCoord, YCoord);
        yield return transform.MoveToSpotCoroutine(newTile.transform.position, TileSlideSpeed, false);

        yield return AfterMove(newTile);
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

    public virtual void SpawnOnGrid(Dungeon dungeon, GridTile tile)
    {
        dungeon.SpawnEntity(this, tile);
    }

    private void OnMouseUp()
    {
        Game.Dungeon.EntityClicked(this);
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
    Amenity = 32,
    Totem = 64,
}

public interface IGeneratesTileEntity
{
    TileEntity InstantiateTileEntity();
}

public interface IGeneratesTileEntity<TTileEntityType> : IGeneratesTileEntity where TTileEntityType : TileEntity
{
    TTileEntityType InstantiateEntity();
}