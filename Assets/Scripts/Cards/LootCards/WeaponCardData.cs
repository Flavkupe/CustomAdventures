using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Item/Weapon Card", order = 1)]
public class WeaponCardData : EquippableItemCardData
{
    public int Power = 1;
    public int Durability = 5;
    public override LootCardType LootCardType { get { return LootCardType.Weapon; } }
    public override Type BackingCardType { get { return typeof(WeaponCard); } }

    public AnimatedEquipment AnimatedObject;

    public AudioClip[] HitSounds;
}
