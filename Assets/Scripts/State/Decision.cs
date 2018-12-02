using Assets.Scripts.State;
using System;

public interface IDecision
{
    bool Evaluate(GameContext context);
}

public interface IDecision<TEventType> where TEventType : struct
{
    bool EvaluateEvent(StateContext<TEventType> context);
    bool EvaluateContext(GameContext gameContext);
}

public abstract class Decision<TEventType> : IDecision<TEventType> where TEventType : struct
{
    /// <summary>
    /// An evaluation that is checked only when an event specific to this state occurs
    /// (event must be of TEventType).
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public abstract bool EvaluateEvent(StateContext<TEventType> context);

    /// <summary>
    /// A general evaluation based on the game context as a whole rather than something 
    /// state-specific. Is always checked.
    /// </summary>
    public virtual bool EvaluateContext(GameContext gameContext)
    {
        return false;
    }
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
    public DidEventOccur(TEventType eventType)
    {
        _eventType = eventType;
    }

    public override bool EvaluateEvent(StateContext<TEventType> context)
    {
        return context.Event.Equals(_eventType);
    }
}

public class LambdaDecision<TEventType> : Decision<TEventType> where TEventType : struct
{
    private Func<GameContext, bool> _contextAction;
    private Func<StateContext<TEventType>, bool> _eventAction;

    public LambdaDecision(Func<GameContext, bool> contextAction, Func<StateContext<TEventType>, bool> eventAction)
    {
        _contextAction = contextAction;
        _eventAction = eventAction;
    }

    public LambdaDecision(Func<GameContext, bool> contextAction) : this(contextAction, null)
    {
    }

    public override bool EvaluateContext(GameContext gameContext)
    {
        if (_contextAction != null)
        {
            return _contextAction(gameContext);
        }

        return false;
    }

    public override bool EvaluateEvent(StateContext<TEventType> context)
    {
        if (_eventAction != null)
        {
            return _eventAction(context);
        }

        return false;
    }
}
