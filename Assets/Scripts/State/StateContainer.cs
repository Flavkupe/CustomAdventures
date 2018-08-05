using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ActionCheckerContainer<TActionType>
{
    public ActionCheckerContainer(params IActionDeterminant<TActionType>[] determinants)
    {
        _actionDeterminants.AddRange(determinants);
    }

    private List<IActionDeterminant<TActionType>> _actionDeterminants = new List<IActionDeterminant<TActionType>>();

    public bool CanPerformAction(TActionType action)
    {
        return _actionDeterminants.All(a => a.CanPerformAction(action));
    }
}

