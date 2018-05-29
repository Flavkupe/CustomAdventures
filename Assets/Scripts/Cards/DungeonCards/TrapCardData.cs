using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Dungeon/Trap Card", order = 1)]
public class TrapCardData : DungeonCardData
{
    public Sprite Sprite;

    public int Level = 1;
    public int Damage = 1;

    public override DungeonCardType DungeonCardType { get { return DungeonCardType.Trap; } }
    public override Type BackingCardType { get { return typeof(TrapCard); } }
}
