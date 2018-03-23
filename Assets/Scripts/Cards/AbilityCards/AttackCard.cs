
using System;
using System.Collections;
using System.Collections.Generic;

public class AttackCard : AbilityCard<AttackCardData>
{
    public override void ActivateAbility()
    {
        switch (this.Data.ActivationType)
        {
            case AbilityActivationType.Instant:
                this.ActivateInstant();
                break;
            case AbilityActivationType.TargetEntity:
                this.ActivateTargeted();
                break;
        }
    }

    private void ActivateTargeted()
    {
        List<TileEntity> entities = Game.Dungeon.GetEntitiesNearPlayer(this.Data.RangeType, this.Data.Range, this.Data.AffectedTargetType);
        List<Tile> tiles = Game.Dungeon.GetTilesNearPlayer(this.Data.RangeType, this.Data.Range);
        tiles.ForEach(a => a.Show(true));
       
        Routine cardUseRoutine = Routine.CreateCancellable(Game.Dungeon.AwaitTargetSelection, entities, 1);
        Routine damageRoutine = Routine.Create(DoAnimationOnAllSelected);
        damageRoutine.Then(() => DamageTargets(Game.Dungeon.SelectedTargets));
        cardUseRoutine.Then(damageRoutine);
        cardUseRoutine.Then(() => 
        {
            this.AfterCardUsed();
            Game.Dungeon.AfterPlayerTurn();
        });

        cardUseRoutine.Finally(() => 
        {
            tiles.ForEach(a => a.Show(false));
            Game.Dungeon.SelectedTargets.Clear();
        });

        cardUseRoutine.OnReject(() => Game.States.SetState(GameState.AwaitingCommand));
        Game.States.EnqueueCoroutine(cardUseRoutine);
    }

    private void ActivateInstant()
    {
        List<TileEntity> entities = Game.Dungeon.GetEntitiesNearPlayer(this.Data.RangeType, this.Data.Range, this.Data.AffectedTargetType);
        ParallelRoutineSet routines = new ParallelRoutineSet();
        foreach (var target in entities)
        {
            routines.AddRoutine(Routine.Create(DoAnimationOnTarget, target));
        }

        Routine animationsRoutine = routines.AsRoutine();
        animationsRoutine.Then(() => 
        {
            DamageTargets(entities);
            this.AfterCardUsed();
        });

        Game.States.EnqueueCoroutine(animationsRoutine);
    }

    private IEnumerator DoAnimationOnTarget(TileEntity entity)
    {
        if (this.Data.AnimationEffect != null)
        {
            var effect = Game.Effects.CreateTargetedAnimationEffect(this.Data.AnimationEffect, entity.transform.position, Game.Player.transform.position);
            yield return effect.CreateRoutine();
        }
    }

    private IEnumerator DoAnimationOnAllSelected()
    {
        List<TileEntity> entities = Game.Dungeon.SelectedTargets;
        foreach (var entity in entities)
        {
            yield return DoAnimationOnTarget(entity);
        }
    }

    private void DamageTargets(List<TileEntity> entities)
    {
        int damage = this.Data.Damage;
        foreach (TileEntity entity in entities)
        {
            entity.DoDamage(damage);
        }
    }
}

