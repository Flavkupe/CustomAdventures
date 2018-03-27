using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "CreateAction Cards/Dungeon/Treasure Card", order = 1)]
public class TreasureCardData : DungeonCardData
{
    public Sprite Sprite;

    public int Level = 1;
    public override DungeonCardType DungeonCardType { get { return DungeonCardType.Treasure; } }
    public override Type BackingCardType { get { return typeof(TreasureCard); } }
}
