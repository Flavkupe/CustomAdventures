public class DungeonStateController : StateController<DungeonStateChangeContext>, IActionDeterminant<DungeonActionType>
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

    public void SendEvent(DungeonEventType eventType, GameContext context)
    {
        var eventContext = new DungeonStateChangeContext(eventType, context);
        EventOccurred(eventContext);
    }

    public bool CanPerformAction(DungeonActionType actionType)
    {
        return CanPerformActionInState(actionType);
    }

    public void SwitchToTileSelection(GameContext context, EntitySelectionOptions options, ActionOnEntities doOnSelected)
    {
        awaitingGridSelection.StartSelection(context, options, doOnSelected);
        ChangeState(awaitingGridSelection, new DungeonStateChangeContext(DungeonEventType.SelectionStarted, context));
    }
}
