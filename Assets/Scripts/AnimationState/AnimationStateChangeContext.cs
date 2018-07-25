public class AnimationStateChangeContext
{
    public AnimationStateChangeContext(AnimationEventType animationEvent, GameContext gameContext)
    {
        EventType = animationEvent;
        GameContext = gameContext;
    }

    public AnimationEventType EventType { get; }

    public GameContext GameContext { get; }
}
