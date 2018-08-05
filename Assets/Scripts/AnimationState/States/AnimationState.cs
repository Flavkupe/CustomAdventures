public class AnimationStateTransition : Transition<AnimationStateChangeContext>
{
    public AnimationStateTransition(IDecision<AnimationStateChangeContext> decision, IState<AnimationStateChangeContext> next)
        : base(decision, next)
    {
    }
}

public abstract class AnimationState : State<AnimationStateChangeContext>, IActionDeterminant<DungeonActionType>
{
    public AnimationState(StateController<AnimationStateChangeContext> contoller) : base(contoller)
    {
    }

    public abstract bool CanPerformAction(DungeonActionType actionType);
}
