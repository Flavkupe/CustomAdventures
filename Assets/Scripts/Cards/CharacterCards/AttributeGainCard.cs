public class AttributeGainCard : CharacterCard<AttributeGainCardData>
{
    public override void ApplyEffect()
    {
        if (Data.StrengthGain > 0)
        {
            Game.Player.Stats.BaseStrength += Data.StrengthGain;
        }

        if (Data.MaxHPGain > 0)
        {
            Game.Player.BaseStats.HP += Data.MaxHPGain;
            Game.Player.Stats.HP += Data.MaxHPGain;
        }
    }
}

