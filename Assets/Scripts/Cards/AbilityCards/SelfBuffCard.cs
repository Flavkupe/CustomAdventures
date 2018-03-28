
public class SelfBuffCard : AbilityCard<SelfBuffCardData>
{
    public override void ActivateAbility()
    {
        foreach (SelfBuff buff in Data.Buffs)
        {
            Game.Player.ApplyEffect(buff);
        }
    }
}

