using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class DungeonDecision : Decision
{
}

public class AreCardsMovingDecision : DungeonDecision
{
    public override bool Evaluate(GameContext context)
    {
        return false;
    }
}
