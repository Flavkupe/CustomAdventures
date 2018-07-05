using System.Collections;

public class AttributeGainCard : CharacterCard<AttributeGainCardData>
{
    public override IEnumerator ApplyEffect(CharacterCardExecutionContext context)
    {
        if (Data.StrengthGain > 0)
        {
            context.Player.CurrentStats.Strength += Data.StrengthGain;
        }

        if (Data.MaxHPGain > 0)
        {
            context.Player.BaseStats.HP += Data.MaxHPGain;
            context.Player.CurrentStats.HP += Data.MaxHPGain;
        }

        yield return null;
    }
}

