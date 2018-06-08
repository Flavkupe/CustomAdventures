using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Dungeon/Enemy Card", order = 1)]
public class EnemyCardData : DungeonCardData
{
    public Sprite Sprite;

    public int Level = 1;
    public int MaxHP = 1;
    public int Movement = 1;
    public int Attack = 1;
    public int EXP = 1;

    public bool Boss = false;

    public int FreeMoves = 1;
    public int FullActions = 1;

    public AudioClip[] DamagedSounds;
    public AudioClip[] DeathSounds;

    public AnimationEffectData DeathEffect;

    public override DungeonCardType DungeonCardType { get { return DungeonCardType.Enemy; } }

    public override Type BackingCardType { get { return typeof(EnemyCard); } }
}
