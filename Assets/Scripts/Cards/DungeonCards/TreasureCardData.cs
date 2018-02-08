using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Treasure Card", order = 1)]
public class TreasureCardData : DungeonCardData
{
    public Sprite Sprite;

    public string Name;
    public int Level = 1;
    public override DungeonCardType DungeonCardType { get { return DungeonCardType.Treasure; } }
}
