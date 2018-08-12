using Assets.Scripts.UI.State.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.UI.State.States
{
    public class UIStateTransition : Transition<UIStateChangeContext>
    {
        public UIStateTransition(IDecision<UIStateChangeContext> decision, IState<UIStateChangeContext> next)
            : base(decision, next)
        {
        }
    }

    public abstract class UIState : State<UIStateChangeContext>, IActionDeterminant<DungeonActionType>
    {
        public UIState(StateController<UIStateChangeContext> contoller) : base(contoller)
        {
        }

        public abstract bool CanPerformAction(DungeonActionType actionType);
    }
}
