using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "Create Behavior/Actions/Follow Player", order = 1)]
public class MoveTowardsPlayerAction : ActorAction
{
    public override IEnumerator PerformAction(TileAI subject, GameContext context)
    {
        // Basic move
        Direction? dir = null;
        if (context.Player.XCoord > subject.XCoord && subject.CanMove(Direction.Right))
        {
            dir = Direction.Right;
        }
        else if (context.Player.XCoord < subject.XCoord && subject.CanMove(Direction.Left))
        {
            dir = Direction.Left;
        }
        else if (context.Player.YCoord > subject.YCoord && subject.CanMove(Direction.Up))
        {
            dir = Direction.Up;
        }
        else if (context.Player.YCoord < subject.YCoord && subject.CanMove(Direction.Down))
        {
            dir = Direction.Down;
        }

        if (dir != null)
        {
            yield return subject.TryMove(dir.Value);
        }
    }
}

