using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IActionDeterminant<in TActionType>
{
    bool CanPerformAction(TActionType actionType);
}

