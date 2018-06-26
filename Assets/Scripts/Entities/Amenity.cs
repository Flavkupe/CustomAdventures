using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class Amenity : TileEntity
{
    private bool _usable = true;

    public AmenityData Data { get; set; }

    public override TileEntityType EntityType => TileEntityType.Amenity;

    public override bool PlayerCanInteractWith()
    {
        return true;
    }

    public override IEnumerator PlayerInteractWith()
    {
        if (_usable && Data.StatusEffects.Length > 0)
        {
            yield return PlayerTwitchTowardsThis();
            yield return UseThis();
            _usable = false;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(Data.CantUseDialogMessage))
            {
                Game.UI.MessageDialog.Show(Data.CantUseDialogMessage);
            }

            var message = Data.StatusEffects.Length == 0 ? "This does nothing!" : Data.CantUseMessage;
            ShowFloatyText(new FloatyTextOptions {
                Text = message,
                Color = Color.white,
                Size = FloatyTextSize.Medium, 
                OnlyShowOnEmptyQueue = true
            });
        }
    }

    private IEnumerator UseThis()
    {
        foreach (var effect in Data.StatusEffects)
        {
            yield return effect.ApplyEffectOn(Game.Player, transform.position);
        }
    }

    public override PlayerInteraction GetPlayerInteraction(Player player)
    {
        return PlayerInteraction.InteractWithObject;
    }

    [UsedImplicitly]
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Data.Sprite;
        GetComponent<SpriteRenderer>().sortingLayerName = "Entities";
    }
}

