using Assets.Scripts.UI.State.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.UI.State.Decisions
{
    public abstract class UIDecision : Decision<UIStateChangeContext>
    {
        public class Decisions
        {
            public static DidEventOccur<UIStateChangeContext, UIEventType> DidEventOccur(UIEventType eventType)
            {
                return new DidEventOccur<UIStateChangeContext, UIEventType>(eventType, a => a.EventType);
            }
        }
    }
}
