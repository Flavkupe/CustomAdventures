using System.Collections;
using UnityEngine;

public abstract class DungeonCard<T> : Card<T>, IDungeonCard where T : DungeonCardData
{
    public override CardType CardType => CardType.Dungeon;

    public TDataType GetData<TDataType>() where TDataType : DungeonCardData
    {
        Debug.Assert(Data is TDataType, "Assumed incorrect data type for DungeonCard.");
        return Data as TDataType;
    }

    public abstract IEnumerator ExecuteDungeonEvent(DungeonCardExecutionContext context);

    public int GetNumberOfExecutions()
    {
        if (Data.EffectRepeatMinTimes == 0 || Data.EffectRepeatMaxTimes == 0 ||
            Data.EffectRepeatMinTimes >= Data.EffectRepeatMaxTimes)
        {
            return 1;
        }
        else
        {
            int repeats = UnityEngine.Random.Range(Data.EffectRepeatMinTimes, Data.EffectRepeatMaxTimes + 1);
            return repeats + 1;
        }
    }

    public virtual bool RequiresFullTile => true;
}

public class DungeonCardExecutionContext
{
    public Dungeon Dungeon { get; }
    public Player Player { get; }
    public RoomArea Area { get; }

    public DungeonCardExecutionContext(Dungeon dungeon, Player player, RoomArea area)
    {
        Dungeon = dungeon;
        Player = player;
        Area = area;
    }
}

public interface IDungeonCard : ICard
{ 
    bool RequiresFullTile { get; }

    int GetNumberOfExecutions();

    TDataType GetData<TDataType>() where TDataType : DungeonCardData;

    IEnumerator ExecuteDungeonEvent(DungeonCardExecutionContext context);
}

public enum DungeonCardType
{
    Enemy,
    Treasure,
    Trap,

    Multi,

    Prop,
    Totem,
}

public abstract class DungeonCardData : CardData
{
    public abstract DungeonCardType DungeonCardType { get; }
    
    [Tooltip("If more than 1, will repeat at least that many times, bounded by EffectRepeatMaxTimes")]
    public int EffectRepeatMinTimes = 0;

    [Tooltip("Upper bound for EffectRepeatMinTimes")]
    public int EffectRepeatMaxTimes = 0;
}
