using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class AttributeGainCard : CharacterCard<AttributeGainCardData>
{
    public override void ApplyEffect()
    {
        if (this.Data.StrengthGain > 0)
        {
            Game.Player.Stats.BaseStrength += this.Data.StrengthGain;
        }

        if (this.Data.MaxHPGain > 0)
        {
            Game.Player.Stats.MaxHP += this.Data.MaxHPGain;
            Game.Player.Stats.HP += this.Data.MaxHPGain;
        }
    }
}

