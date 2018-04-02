using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Treasure : TileEntity
{
    public TreasureCardData Data { get; set; }

    public override TileEntityType EntityType
    {
        get
        {
            return TileEntityType.Environment;
        }
    }

    private bool _canInteractWith = true;

    //protected override void OnClicked()
    //{
    //    //Game.UI.UpdateEntityPanel(this);
    //    base.OnClicked();
    //}

    [UsedImplicitly]
    private void Start ()
    {
        GetComponent<SpriteRenderer>().sprite = Data.Sprite;
        GetComponent<SpriteRenderer>().sortingLayerName = "Entities";
    }

    public override bool PlayerCanInteractWith()
    {
        return _canInteractWith;
    }

    public override PlayerInteraction GetPlayerInteraction(Player player)
    {
        return PlayerInteraction.InteractWithObject;
    }

    public override IEnumerator PlayerInteractWith()
    {
        var playerDirection = Game.Player.transform.position.GetRelativeDirection(transform.position);
        yield return Game.Player.TwitchTowards(playerDirection);
        Game.CardDraw.PerformLootCardDrawing(this.Data.NumTreasures);
        _canInteractWith = false;
        Destroy(gameObject, 1.0f);
    }
}
