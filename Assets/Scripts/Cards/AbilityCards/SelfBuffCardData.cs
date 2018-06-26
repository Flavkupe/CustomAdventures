

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Ability/Self Buff Card", order = 1)]
public class SelfBuffCardData : AbilityCardData
{
    public override AbilityCardType AbilityCardType => AbilityCardType.SelfBuff;

    public PersistentEffectData[] Buffs;

    public override Type BackingCardType => typeof(SelfBuffCard);
}

