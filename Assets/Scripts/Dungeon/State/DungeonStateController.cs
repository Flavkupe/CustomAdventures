using Assets.Scripts.State;

public class DungeonStateController : StateController<DungeonState, DungeonEventType>, IActionDeterminant<DungeonActionType>
{
    private readonly AwaitingInputState awaitingInputState;
    private readonly AwaitingAIState awaitingAIState;
    private readonly AwaitingGridSelectionState awaitingGridSelection;

    public DungeonStateController() : base("Dungeon")
    {
        awaitingInputState = new AwaitingInputState(this);
        awaitingAIState = new AwaitingAIState(this);
        awaitingGridSelection = new AwaitingGridSelectionState(this);

        awaitingInputState.AddTransitions(new[]
        {
            new DungeonStateTransition(DungeonDecision.Decisions.DidEnemyTurnStart, awaitingAIState)
        });

        awaitingAIState.AddTransitions(new[]
        {
            new DungeonStateTransition(DungeonDecision.Decisions.DidEnemyTurnEnd, awaitingInputState)
        });

        awaitingGridSelection.AddTransitions(new[]
        {
            new DungeonStateTransition(DungeonDecision.Decisions.DidEventOccur(DungeonEventType.SelectionCancelled), awaitingInputState),
            new DungeonStateTransition(DungeonDecision.Decisions.DidEventOccur(DungeonEventType.SelectionCompleted), awaitingInputState),
        });

        FirstState = awaitingInputState;
        CurrentState = FirstState;
    }

    public bool CanPerformAction(DungeonActionType actionType)
    {
        return CanPerformActionInState(actionType);
    }

    public void SwitchToTileSelection(GameContext context, EntitySelectionOptions options, ActionOnEntities doOnSelected)
    {
        awaitingGridSelection.StartSelection(context, options, doOnSelected);
        ChangeState(awaitingGridSelection, new StateContext<DungeonEventType>(DungeonEventType.SelectionStarted, context, CurrentState));
    }
}
