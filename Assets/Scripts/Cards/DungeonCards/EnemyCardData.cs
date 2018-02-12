using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Enemy Card", order = 1)]
public class EnemyCardData : DungeonCardData
{
    public Sprite Sprite;

    public string Name;
    public int Level = 1;
    public int MaxHP = 1;
    public int Movement = 1;
    public int Attack = 1;
    public int EXP = 1;

    public override DungeonCardType DungeonCardType { get { return DungeonCardType.Enemy; } }

    public override Type BackingCardType { get { return typeof(EnemyCard); } }
}
