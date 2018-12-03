
public class SelfBuffCard : AbilityCard<SelfBuffCardData>
{
    public override void ActivateAbility(GameContext context)
    {
        foreach (var buff in Data.Buffs)
        {
            var routine = Routine.Create(buff.ApplyEffectOn, context.Player, context.Player);
            context.Dungeon.EnqueueAnimation(routine);
        }
    }
}

