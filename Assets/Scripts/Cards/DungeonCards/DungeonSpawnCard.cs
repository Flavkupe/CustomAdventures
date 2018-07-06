using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DungeonSpawnCard<TDataType, TEntityType> : DungeonCard<TDataType> 
    where TDataType : EntityCardData<TEntityType> 
    where TEntityType : TileEntity
{
    public sealed override IEnumerator ExecuteDungeonEvent(DungeonCardExecutionContext context)
    {
        context.Dungeon.PauseActions();

        var numTimes = GetNumberOfExecutions();

        var preExecutionRoutines = new RoutineChain();
        var executionRoutines = new ParallelRoutineSet();
        for (var i = 0; i < numTimes; i++)
        {
            // Which room does the event happen in?
            var spawnRoomArea = context.Area;
            if (Data.RoomEventType == RoomEventType.RandomUnexplored)
            {
                spawnRoomArea = context.Dungeon.GetUnexploredRoomArea(spawnRoomArea);
            }

            var newContext = new DungeonCardExecutionContext(context.Dungeon, context.Player, spawnRoomArea);
            var tile = GetTargetTile(spawnRoomArea);
            if (tile != null)
            {
                // To ensure we don't reuse this tile
                tile.Reserve();

                // Queue up pre-execute events for each tile
                var preExecRoutine = Routine.Create(BeforeExecuteSpawnEvent, tile, newContext);
                preExecutionRoutines.AddRoutine(preExecRoutine);

                var cardEffectRoutine = Routine.Create(ExecuteSpawnEvent, tile, newContext);
                if (Data.RoomEventType == RoomEventType.CurrentRoom)
                { 
                    // Card travel effect
                    var executionRoutine = Routine.Create(AnimateCardMoveToEffect, tile, context);
                    executionRoutine.Then(cardEffectRoutine);
                    executionRoutines.AddRoutine(executionRoutine);
                }
                else
                {
                    // No travel effect, so just do event
                    executionRoutines.AddRoutine(cardEffectRoutine);
                }
            }
            else
            {
                // TODO: no tile chosen? What do?
            }
        }

        // First reserve all the tiles and such
        yield return preExecutionRoutines.AsRoutine();

        context.Dungeon.UnpauseActions();

        var defaultAnimation = GetDefaultCardTriggerEffect();
        yield return AnimateCardTriggerEffect(defaultAnimation);

        yield return executionRoutines.AsRoutine();               
    }

    /// <summary>
    /// Events that happen before a card triggers and travels. Should perform
    /// tasks such as reserving tiles.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator BeforeExecuteSpawnEvent(GridTile tile, DungeonCardExecutionContext context)
    {
        tile.Reserve();
        yield return null;
    }

    protected virtual IEnumerator ExecuteSpawnEvent(GridTile tile, DungeonCardExecutionContext context)
    {
        // TODO: animate?
        var entity = Data.InstantiateEntity();
        entity.SpawnOnGrid(context.Dungeon, tile);
        yield return null;
    }

    private IEnumerator AnimateCardMoveToEffect(GridTile tile, DungeonCardExecutionContext context)
    {
        // Card travel effect
        var effect = Data.CardMoveToEffect ?? context.Dungeon.Templates.CardParts.Effects.DefaultCardMoveToEffect;
        if (effect != null)
        {
            var cardTravelEffect = effect.CreateTargetedEffect(tile.transform.position, transform.position);
            yield return cardTravelEffect.CreateRoutine();
        }
    }

    private GridTile GetTargetTile(RoomArea roomArea)
    {
        var tiles = new List<GridTile>();
        if (Data.SpawnType == SpawnEventType.SpawnNear)
        {
            tiles = roomArea.GetAreaTiles();
            
        }
        else if (Data.SpawnType == SpawnEventType.SpawnOnCorner)
        {
            tiles = roomArea.GetCornerTiles();            
        }
        else if (Data.SpawnType == SpawnEventType.SpawnOnWideOpen)
        {
            tiles = roomArea.GetWideOpenTiles();

            if (tiles.Count == 0)
            {
                // Fall back to corner tiles
                tiles = roomArea.GetCornerTiles();
            }
        }

        return tiles.Where(a => a.CanOccupy()).ToList().GetRandom();
    }
}

public abstract class EntityCardData : DungeonCardData, IGeneratesTileEntity
{
    public abstract TileEntity InstantiateTileEntity();

    public SpawnEventType SpawnType;
    public RoomEventType RoomEventType;
    public TargetedAnimationEffectData CardMoveToEffect;
}

/// <summary>
/// What room will an event occur in?
/// </summary>
public enum RoomEventType
{
    CurrentRoom,
    RandomUnexplored,
}

public enum SpawnEventType
{
    SpawnNear,
    SpawnOnCorner,
    SpawnOnWideOpen,
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