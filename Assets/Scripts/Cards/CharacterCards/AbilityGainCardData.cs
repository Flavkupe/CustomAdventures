using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Character Card/Ability Gain Card", order = 1)]
public class AbilityGainCardData : CharacterCardData
{
    public override Type BackingCardType
    {
        get
        {
            return typeof(AbilityGainCard);
        }
    }

    public override CharacterCardType CharacterCardType
    {
        get
        {
            return CharacterCardType.AbilityGain;
        }
    }

    public int NumberGained = 1;
    public AbilityCardData AbilityGained;
}

