namespace Assets.Scripts.UI.State.States
{
    public class IdleUIState : UIState
    {
        public IdleUIState(IStateController<UIEventType> contoller) : base(contoller)
        {
        }

        public override bool CanPerformAction(DungeonActionType actionType)
        {
            switch (actionType)
            {
                case DungeonActionType.PlayerMove:
                case DungeonActionType.UseItem:
                case DungeonActionType.EntityMove:
                case DungeonActionType.OpenMenu:
                case DungeonActionType.PerformCardDraw:
                default:
                    return true;
            }
        }
    }
}
