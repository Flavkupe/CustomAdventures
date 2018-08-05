using System;

public interface IDecision<in TChangeContext>
{
    bool Evaluate(TChangeContext context);
}

public abstract class Decision<TChangeContext> : IDecision<TChangeContext>
{
    public abstract bool Evaluate(TChangeContext context);
}

public class DidEventOccur<TChangeContext, TEventType> : Decision<TChangeContext> where TEventType : struct
{
    private TEventType _eventType;
    private Func<TChangeContext, TEventType> _eventAccessorFunc;
    public DidEventOccur(TEventType eventType, Func<TChangeContext, TEventType> eventAccessorFunc)
    {
        _eventType = eventType;
        _eventAccessorFunc = eventAccessorFunc;
    }

    public override bool Evaluate(TChangeContext context)
    {
        return _eventAccessorFunc(context).Equals(_eventType);
    }
}
