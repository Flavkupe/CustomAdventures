using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SoundGenerator))]
public class CurseTotem : TileEntity
{
    public TotemCardData Data { get; set; }

    private SoundGenerator _soundGen;

    private SpriteRenderer _renderer;

    public override TileEntityType EntityType => TileEntityType.Totem;

    private bool _canInteractWith = true;

    [UsedImplicitly]
    private void Start ()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = Data.Sprite;
        _renderer.sortingLayerName = "Entities";
        _soundGen = GetComponent<SoundGenerator>();
    }

    public override bool PlayerCanInteractWith()
    {
        return _canInteractWith;
    }

    public override PlayerInteraction GetPlayerInteraction(Player player)
    {
        return PlayerInteraction.InteractWithObject;
    }

    public override IEnumerator PlayerInteractWith(Player player)
    {
        yield return PlayerTwitchTowardsThis();

        _soundGen.PlayRandomFrom(Data.BreakSounds);

        // Find and expire matching effect
        var matchingEffect = player.Effects.FirstOrDefault(a => a.GetIdentifier() == Data.CurseEffect.GetIdentifier());
        matchingEffect?.Expire(player);

        _renderer.enabled = false;
        _canInteractWith = false;
        Destroy(gameObject, 1.0f);
    }
}
