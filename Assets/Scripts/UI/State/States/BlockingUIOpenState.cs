using Assets.Scripts.UI.State.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.UI.State.States
{
    public class BlockingUIOpenState : UIState
    {
        public BlockingUIOpenState(IStateController<UIStateChangeContext> contoller) : base(contoller)
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
