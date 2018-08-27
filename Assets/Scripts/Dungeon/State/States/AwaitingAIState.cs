using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// State where the AI animation is being awaited, such as enemy movement.
/// </summary>
public class AwaitingAIState : DungeonState
{
    public AwaitingAIState(StateController<DungeonStateChangeContext> contoller) : base(contoller)
    {
    }

    public override void StateEntered(IState<DungeonStateChangeContext> previousState, DungeonStateChangeContext context)
    {
        EnqueueEnemyTurns(context);
    }

    public override bool CanPerformAction(DungeonActionType actionType)
    {
        switch (actionType)
        {
            case DungeonActionType.PlayerMove:
            case DungeonActionType.EntityMove:
            case DungeonActionType.UseItem:
            case DungeonActionType.PerformCardDraw:
                return false;
            default:
                return true;
        }
    }

    private void EnqueueEnemyTurns(DungeonStateChangeContext context)
    {
        var enemies = context.GameContext.Dungeon.Enemies;
        var enemyTurns = new RoutineChain(enemies.Select(a => Routine.Create(a.ProcessCharacterTurn)).ToArray());
        enemyTurns.Then(() =>
        {
            RaiseEventOccurred(DungeonEventType.AllEnemiesTurnEnd, context.GameContext);
        });

        EnqueueCoroutine(enemyTurns);
    }
}

