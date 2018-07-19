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

    public override void ItemDurabilityExpended()
    {
        base.ItemDurabilityExpended();

        _durability--;
        if (_durability <= 0)
        {
            Game.Player.DestroyItem(this);
        }
        else
        {
            ItemBroke();
        }
    }

    public override float DurabilityRatio => Mathf.Clamp(_durability / (float)Data.Durability, 0.0f, 1.0f);

    public override bool ShowDurability => true;

    public override int DefenseValue => Data.Blocking;

    public override void ItemEquipped()
    {
        Game.Sounds.PlayFromClips(Data.EquipSounds, Game.Sounds.DefaultItemPickupSounds.GetRandom());
    }

    public override void ItemBroke()
    {
        Game.Sounds.PlayFromClips(Data.BreakSounds);
    }
}