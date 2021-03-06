﻿using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Ability/Attack Card", order = 1)]
public class AttackCardData : AbilityCardData
{
    public override AbilityCardType AbilityCardType => AbilityCardType.Attack;

    public AttackTargetType TargetType;
    public TileRangeType RangeType;

    public TileEntityType AffectedTargetType = TileEntityType.Enemy;

    public int Damage = 1;
    public int Range = 1;

    public TargetedAnimationEffectData AnimationEffect;

    public override Type BackingCardType => typeof(AttackCard);
}

public enum AttackTargetType
{
    SingleTarget,
    All,
}
