using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Item/Weapon Card", order = 1)]
public class WeaponCardData : ItemCardData
{
    public int Power = 1;
    public int Durability = 5;
    public override LootCardType LootCardType { get { return LootCardType.Weapon; } }
    public override Type BackingCardType { get { return typeof(WeaponCard); } }
}
