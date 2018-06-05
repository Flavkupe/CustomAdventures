using UnityEngine;
public class Armor : InventoryItem<ArmorCardData>
{
    private int _durability;

    public Armor(ArmorCardData data) : base(data)
    {
        _durability = data.Durability;
    }

    public override InventoryItem CreateClone()
    {
        return new Armor(Data);
    }

    public override void ItemUsed()
    {
        base.ItemUsed();

        _durability--;
        if (_durability <= 0)
        {
            Game.Player.DestroyItem(this);
        }
        else
        {
            Game.Sounds.PlayFromClips(Data.BreakSounds);
        }
    }

    public override float DurabilityRatio
    {
        get { return Mathf.Clamp(_durability / (float)Data.Durability, 0.0f, 1.0f); }
    }

    public override bool ShowDurability
    {
        get { return true; }
    }

    public override int DefenseValue
    {
        get 
        {
            return Data.Blocking;
        }
    }

    public override void ItemEquipped()
    {
        Game.Sounds.PlayFromClips(Data.EquipSounds, Game.Sounds.DefaultItemPickupSounds.GetRandom());
    }

    public override void ItemBroken()
    {
        Game.Sounds.PlayFromClips(Data.BreakSounds);
    }
}