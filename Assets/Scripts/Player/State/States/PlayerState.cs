
using Assets.Scripts.Player.State.Context;

public class PlayerStateTransition : Transition<PlayerStateChangeContext>
{
    public PlayerStateTransition(IDecision<PlayerStateChangeContext> decision, IState<PlayerStateChangeContext> next)
        : base(decision, next)
    {
    }
}

public abstract class PlayerState : State<PlayerStateChangeContext>, IActionDeterminant<DungeonActionType>
{
    public abstract bool CanPerformAction(DungeonActionType actionType);

    protected void RaiseEventOccurred(PlayerEventType newEvent, DungeonStateChangeContext context)
    {
        RaiseEventOccurred(new PlayerStateChangeContext(newEvent, context.GameContext));
    }

    public sealed override void Update(GameContext context)
    {
        // TODO: narrow down...?
    }
}