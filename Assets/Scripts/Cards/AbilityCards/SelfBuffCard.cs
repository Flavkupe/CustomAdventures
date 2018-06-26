
public class SelfBuffCard : AbilityCard<SelfBuffCardData>
{
    public override void ActivateAbility()
    {
        foreach (var buff in Data.Buffs)
        {
            StartCoroutine(buff.ApplyEffectOn(Game.Player, Game.Player));
        }
    }
}

