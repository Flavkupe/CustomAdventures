

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "CreateAction Cards/Ability/Self Buff Card", order = 1)]
public class SelfBuffCardData : AbilityCardData
{
    public override AbilityCardType AbilityCardType
    {
        get
        {
            return AbilityCardType.SelfBuff;
        }
    }

    public SelfBuff[] Buffs;

    public override Type BackingCardType { get { return typeof(SelfBuffCard); } }
}

