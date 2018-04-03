using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Item/Earn Gold Card", order = 1)]
public class EarnGoldCardData : ItemCardData
{
    public int MinRange = 1;
    public int MaxRange = 10;

    public override LootCardType LootCardType { get { return LootCardType.Gold; } }

    public override Type BackingCardType { get { return typeof(EarnGoldCard); } }
}
