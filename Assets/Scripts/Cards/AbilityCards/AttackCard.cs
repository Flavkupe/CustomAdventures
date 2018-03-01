
using System;
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

        Routine routine = Routine.CreateCancellable(Game.Dungeon.AwaitTargetSelection, entities, 1);
        routine.Then(() => DamageTargets(Game.Dungeon.SelectedTargets)).Then(() => {
            this.AfterCardUsed();
            Game.Dungeon.AfterPlayerTurn();            
        });
        routine.Finally(() => tiles.ForEach(a => a.Show(false)));

        routine.OnReject(() => Game.States.SetState(GameState.AwaitingCommand));
        Game.States.EnqueueRoutine(routine);
    }

    private void ActivateInstant()
    {
        List<TileEntity> entities = DungeonManager.Instance.GetEntitiesNearPlayer(this.Data.RangeType, this.Data.Range, this.Data.AffectedTargetType);
        DamageTargets(entities);
        this.AfterCardUsed();
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

