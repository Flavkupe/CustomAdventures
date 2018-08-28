public class AnimationStateTransition : Transition<AnimationStateChangeContext>
{
    public AnimationStateTransition(IDecision<AnimationStateChangeContext> decision, IState<AnimationStateChangeContext> next)
        : base(decision, next)
    {
    }
}

public abstract class AnimationState : State<AnimationStateChangeContext>, IActionDeterminant<DungeonActionType>
{
    protected AnimationState(IStateController<AnimationStateChangeContext> contoller) : base(contoller)
    {
    }

    public abstract bool CanPerformAction(DungeonActionType actionType);
}
