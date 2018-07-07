using UnityEngine;

public class Weapon : InventoryItem<WeaponCardData>
{
    private int _durability;

    public Weapon(WeaponCardData data) : base(data)
    {
        _durability = data.Durability;
    }

    public override InventoryItem CreateClone()
    {
        return new Weapon(Data);
    }

    public override void ItemUsed()
    {
        base.ItemUsed();

        _durability--;
        if (_durability <= 0)
        {
            Game.Player.DestroyItem(this);
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

    public override void ItemEquipped()
    {
        Game.Sounds.PlayFromClips(Data.EquipSounds, Game.Sounds.DefaultItemPickupSounds.GetRandom());
        Game.Player.SetAnimatedWeapon(Data.AnimatedObject);
    }

    public override void ItemUnequipped()
    {
        Game.Player.SetAnimatedWeapon(null);
    }

    public override void PlayItemBrokenSound()
    {
        Game.Sounds.PlayFromClips(Data.BreakSounds);
        Game.Player.SetAnimatedWeapon(null);
    }
}

