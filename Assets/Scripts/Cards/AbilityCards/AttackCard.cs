using System.Collections;
using System.Collections.Generic;

public class AttackCard : AbilityCard<AttackCardData>
{
    public override void ActivateAbility(GameContext context)
    {
        switch (Data.ActivationType)
        {
            case AbilityActivationType.Instant:
                ActivateInstant(context);
                break;
            case AbilityActivationType.TargetEntity:
                ActivateTargeted(context);
                break;
        }
    }

    private void ActivateTargeted(GameContext context)
    {
        List<TileEntity> entities = context.Dungeon.GetEntitiesNearPlayer(Data.RangeType, Data.Range, Data.AffectedTargetType);
        List<GridTile> tiles = context.Dungeon.GetTilesNearPlayer(Data.RangeType, Data.Range);
        tiles.ForEach(a => a.Show(true));

        context.Dungeon.PerformGridSelection(entities, new EntitySelectionOptions(1, entities), DoDamageRoutine);

        Routine cardUseRoutine = Routine.CreateCancellable(Game.Dungeon.AwaitTargetSelection, entities, 1);

        

        cardUseRoutine.Then(damageRoutine);
        cardUseRoutine.Then(() => 
        {
            AfterCardUsed();
        });

        cardUseRoutine.Finally(() => 
        {
            tiles.ForEach(a => a.Show(false));
            Game.Dungeon.SelectedTargets.Clear();
        });

        cardUseRoutine.OnReject(() => Game.States.SetState(GameState.AwaitingCommand));
        Game.States.EnqueueCoroutine(cardUseRoutine);
    }

    private void ActivateInstant(GameContext context)
    {
        List<TileEntity> entities = Game.Dungeon.GetEntitiesNearPlayer(Data.RangeType, Data.Range, Data.AffectedTargetType);
        ParallelRoutineSet routines = new ParallelRoutineSet();
        foreach (var target in entities)
        {
            routines.AddRoutine(Routine.Create(DoAnimationOnTarget, target));
        }

        Routine animationsRoutine = routines.AsRoutine();
        animationsRoutine.Then(() => 
        {
            DamageTargets(entities);
            AfterCardUsed();
        });

        Game.States.EnqueueCoroutine(animationsRoutine);
    }

    private IEnumerator DoDamageRoutine(List<TileEntity> entities)
    {
        yield return DoAnimationOnAll(entities);
        DamageTargets(entities);

    }

    private IEnumerator DoAnimationOnTarget(TileEntity entity)
    {
        if (Data.AnimationEffect != null)
        {
            var effect = Data.AnimationEffect.CreateTargetedEffect(entity.transform.position, Game.Player.transform.position);
            yield return effect.CreateRoutine();
        }
    }

    private IEnumerator DoAnimationOnAll(List<TileEntity> entities)
    {
        foreach (var entity in entities)
        {
            yield return DoAnimationOnTarget(entity);
        }
    }

    private void DamageTargets(List<TileEntity> entities)
    {
        int damage = Data.Damage;
        foreach (TileEntity entity in entities)
        {
            entity.DoDamage(damage);
        }
    }
}

