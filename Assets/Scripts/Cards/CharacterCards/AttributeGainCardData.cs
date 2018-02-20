using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Character Card/Attribute Gain Card", order = 1)]
public class AttributeGainCardData : CharacterCardData
{
    public override Type BackingCardType
    {
        get
        {
            return typeof(AttributeGainCard);
        }
    }

    public override CharacterCardType CharacterCardType
    {
        get
        {
            return CharacterCardType.AttributeGain;
        }
    }

    public int MaxHPGain = 0;
    public int StrengthGain = 0;
    public int ArmorGain = 0;
}

