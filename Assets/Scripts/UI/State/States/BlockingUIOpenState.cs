namespace Assets.Scripts.UI.State.States
{
    public class BlockingUIOpenState : UIState
    {
        public BlockingUIOpenState(IStateController<UIEventType> contoller) : base(contoller)
        {
        }

        public override bool CanPerformAction(DungeonActionType actionType)
        {
            switch (actionType)
            {
                case DungeonActionType.PlayerMove:
                case DungeonActionType.EntityMove:
                case DungeonActionType.OpenMenu:
                case DungeonActionType.UseAbility:
                case DungeonActionType.PerformCardDraw:
                default:
                    return false;
                case DungeonActionType.UseItem:
                    return true;
            }
        }
    }
}
