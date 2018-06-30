using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Create Cards/Dungeon/Trap Card", order = 1)]
public class TrapCardData : EntityCardData<TileTrap>
{
    public Sprite Sprite;

    public int Level = 1;
    public int Damage = 1;

    public override DungeonCardType DungeonCardType => DungeonCardType.Trap;
    public override Type BackingCardType => typeof(TrapCard);

    public override TileTrap InstantiateEntity()
    {
        var trap = Utils.InstantiateOfType<TileTrap>();
        trap.Data = this;
        return trap;
    }
}
