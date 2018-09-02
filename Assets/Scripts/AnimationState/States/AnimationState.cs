public class AnimationStateTransition : Transition<AnimationEventType>
{
    public AnimationStateTransition(IDecision<AnimationEventType> decision, IState<AnimationEventType> next)
        : base(decision, next)
    {
    }
}

public abstract class AnimationState : State<AnimationEventType>, IActionDeterminant<DungeonActionType>
{
    protected AnimationState(IStateController<AnimationEventType> contoller) : base(contoller)
    {
    }

    public abstract bool CanPerformAction(DungeonActionType actionType);
}
