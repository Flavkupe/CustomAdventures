using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "Create Behavior/Actions/Idle", order = 1)]
public class IdleAction : ActorAction
{
    public override IEnumerator PerformAction(TileAI subject, GameContext context)
    {
        // Does nothing
        yield break;
    }
}

