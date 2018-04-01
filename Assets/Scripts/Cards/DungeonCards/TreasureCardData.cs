using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Dungeon/Treasure Card", order = 1)]
public class TreasureCardData : DungeonCardData
{
    public Sprite Sprite;

    public int Level = 1;
    public override DungeonCardType DungeonCardType { get { return DungeonCardType.Treasure; } }
    public override Type BackingCardType { get { return typeof(TreasureCard); } }
}
