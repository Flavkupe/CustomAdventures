public class AttributeGainCard : CharacterCard<AttributeGainCardData>
{
    public override void ApplyEffect()
    {
        if (Data.StrengthGain > 0)
        {
            Game.Player.CurrentStats.Strength += Data.StrengthGain;
        }

        if (Data.MaxHPGain > 0)
        {
            Game.Player.BaseStats.HP += Data.MaxHPGain;
            Game.Player.CurrentStats.HP += Data.MaxHPGain;
        }
    }
}

