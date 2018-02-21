
using System.Collections.Generic;

public class AttackCard : AbilityCard<AttackCardData>
{
    public override void ActivateAbility()
    {
        List<TileEntity> entities = DungeonManager.Instance.GetEntitiesNearPlayer(this.Data.RangeType, this.Data.Damage, this.Data.AffectedTargetType);
        int damage = this.Data.Damage;
        foreach (TileEntity entity in entities)
        {
            entity.DoDamage(damage);
        }
    }

    public override void ActivateAbility(Tile target)
    {
        // TODO
        base.ActivateAbility(target);
    }

}

