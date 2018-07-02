using System.Collections;
using UnityEngine;

public abstract class ActorAction : ScriptableObject
{
    public abstract IEnumerator PerformAction(TileAI subject, GameContext context);
}

