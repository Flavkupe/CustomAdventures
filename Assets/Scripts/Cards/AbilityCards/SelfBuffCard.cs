
public class SelfBuffCard : AbilityCard<SelfBuffCardData>
{
    public override void ActivateAbility(GameContext context)
    {
        foreach (var buff in Data.Buffs)
        {
            StartCoroutine(buff.ApplyEffectOn(context.Player, context.Player));
        }
    }
}

