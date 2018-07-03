using UnityEngine;

public abstract class DungeonCard<T> : Card<T>, IDungeonCard where T : DungeonCardData
{
    public override CardType CardType => CardType.Dungeon;

    public abstract void ExecuteTileSpawnEvent(GridTile tile, DungeonCardExecutionContext context);
    public TDataType GetData<TDataType>() where TDataType : DungeonCardData
    {
        Debug.Assert(Data is TDataType, "Assumed incorrect data type for DungeonCard.");
        return Data as TDataType;
    }

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

    public DungeonEventType DungeonEventType => Data.DungeonEventType;

    public RoomEventType RoomEventType => Data.RoomEventType;

    public virtual bool RequiresFullTile => true;
}

public class DungeonCardExecutionContext
{
    public Dungeon Dungeon;
    public Player Player;
}

public abstract class DungeonSpawnCard<TDataType, TEntityType> : DungeonCard<TDataType> 
    where TDataType : EntityCardData<TEntityType> 
    where TEntityType : TileEntity
{
    public override void ExecuteTileSpawnEvent(GridTile tile, DungeonCardExecutionContext context)
    {
        TEntityType entity = Data.InstantiateEntity();
        entity.SpawnOnGrid(Game.Dungeon, tile);
    }
}

public interface IDungeonCard : ICard
{
    DungeonEventType DungeonEventType { get; }

    RoomEventType RoomEventType { get; }

    bool RequiresFullTile { get; }

    void ExecuteTileSpawnEvent(GridTile tile, DungeonCardExecutionContext context);

    int GetNumberOfExecutions();

    TDataType GetData<TDataType>() where TDataType : DungeonCardData;
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

public enum DungeonEventType
{
    SpawnNear,
    SpawnOnCorner,
    SpawnOnWideOpen,

    MultiEvent,
}

/// <summary>
/// What room will an event occur in?
/// </summary>
public enum RoomEventType
{
    CurrentRoom,
    RandomUnexplored,
}

public abstract class DungeonCardData : CardData
{
    public abstract DungeonCardType DungeonCardType { get; }
    public DungeonEventType DungeonEventType;
    public RoomEventType RoomEventType;

    [Tooltip("If more than 1, will repeat at least that many times, bounded by EffectRepeatMaxTimes")]
    public int EffectRepeatMinTimes = 0;

    [Tooltip("Upper bound for EffectRepeatMinTimes")]
    public int EffectRepeatMaxTimes = 0;
}

public abstract class EntityCardData : DungeonCardData, IGeneratesTileEntity
{
    public abstract TileEntity InstantiateTileEntity();
}

public abstract class EntityCardData<TTileEntityType> 
    : EntityCardData, IGeneratesTileEntity<TTileEntityType> where TTileEntityType : TileEntity
{
    public abstract TTileEntityType InstantiateEntity();

    public sealed override TileEntity InstantiateTileEntity()
    {
        return InstantiateEntity();
    }
}