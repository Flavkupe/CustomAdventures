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
            Player.Instance.Stats.BaseStrength += this.Data.StrengthGain;
        }

        if (this.Data.MaxHPGain > 0)
        {
            Player.Instance.Stats.MaxHP += this.Data.MaxHPGain;
            Player.Instance.Stats.HP += this.Data.MaxHPGain;
        }
    }
}

