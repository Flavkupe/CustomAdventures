public class NoAnimationState : AnimationState
{
    public NoAnimationState(StateController<AnimationStateChangeContext> contoller) : base(contoller)
    {
    }

    public override bool CanPerformAction(DungeonActionType actionType)
    {
        switch (actionType)
        {
            case DungeonActionType.PlayerMove:
            case DungeonActionType.EntityMove:
            case DungeonActionType.UseItem:
            default:
                return true;
        }
    }
}