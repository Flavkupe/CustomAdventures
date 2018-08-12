using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.UI.State.Context
{
    public enum UIEventType
    {
        InterfaceOpened,
        InterfaceClosed,
        DialogShown,
        DialogClosed,
    }

    public class UIStateChangeContext
    {
        public UIStateChangeContext(UIEventType eventType, GameContext context)
        {
            EventType = eventType;
            GameContext = context;
        }

        public UIEventType EventType { get; }
        public GameContext GameContext { get; }
    }
}
