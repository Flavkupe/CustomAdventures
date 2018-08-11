public class AnimationStateChangeContext
{
    public AnimationStateChangeContext(AnimationEventType animationEvent, GameContext gameContext, AnimationStateController controller)
    {
        EventType = animationEvent;
        GameContext = gameContext;
        Controller = controller;
    }

    public AnimationEventType EventType { get; }

    public GameContext GameContext { get; }

    public AnimationStateController Controller { get; }
}
