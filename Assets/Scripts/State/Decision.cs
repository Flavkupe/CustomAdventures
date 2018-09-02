using Assets.Scripts.State;
using System;

public interface IDecision
{
    bool Evaluate(GameContext context);
}

public interface IDecision<TEventType> where TEventType : struct
{
    bool Evaluate(StateContext<TEventType> context);
}

public abstract class Decision<TEventType> : IDecision<TEventType> where TEventType : struct
{
    public abstract bool Evaluate(StateContext<TEventType> context);
}

public class Decisions<TEventType> where TEventType : struct
{
    public static DidEventOccur<TEventType> DidEventOccur(TEventType eventType)
    {
        return new DidEventOccur<TEventType>(eventType);
    }
}

public class DidEventOccur<TEventType> : Decision<TEventType> where TEventType : struct
{
    private TEventType _eventType;
    private Func<TEventType, TEventType> _eventAccessorFunc;
    public DidEventOccur(TEventType eventType)
    {
        _eventType = eventType;
    }

    public override bool Evaluate(StateContext<TEventType> context)
    {
        return context.Event.Equals(_eventType);
    }
}
