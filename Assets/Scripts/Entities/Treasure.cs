using System;
using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SoundGenerator))]
public class Treasure : TileEntity, IProducesLootEvent
{
    public TreasureCardData Data { get; set; }

    private SoundGenerator _soundGen;

    public override TileEntityType EntityType
    {
        get
        {
            return TileEntityType.Environment;
        }
    }

    private bool _canInteractWith = true;

    public event EventHandler<LootEventProperties> LootEventRequested;

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
        _soundGen = GetComponent<SoundGenerator>();
    }

    public override void SpawnOnGrid(Dungeon dungeon, GridTile tile)
    {
        dungeon.SpawnLootableEntity(this, tile);
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

        _soundGen.PlayRandomFrom(Data.OpenSounds);
        LootEventRequested?.Invoke(this, Data.LootProperties);
        _canInteractWith = false;
        Destroy(gameObject, 1.0f);
    }
}
