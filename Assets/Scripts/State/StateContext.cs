using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.State
{
    public class StateContext
    {
        public StateContext(GameContext context)
        {
            GameContext = context;
        }

        public GameContext GameContext { get; }
    }

    public class StateContext<TEventType> : StateContext where TEventType : struct
    {
        public TEventType Event { get; }

        public IState<TEventType> CurrentState { get; }

        public StateContext(TEventType newEvent, GameContext context, IState<TEventType> state)
            : base(context)
        {
            Event = newEvent;
            CurrentState = state;
        }
    }
}
