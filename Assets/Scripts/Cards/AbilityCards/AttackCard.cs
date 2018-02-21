
using System.Collections.Generic;

public class AttackCard : AbilityCard<AttackCardData>
{
    public override void ActivateAbility()
    {
        List<TileEntity> entities = DungeonManager.Instance.GetEntitiesNearPlayer(this.Data.RangeType, this.Data.Damage, this.Data.AffectedTargetType);

    }

    public override void ActivateAbility(Tile target)
    {
        // TODO
        base.ActivateAbility(target);
    }

}

