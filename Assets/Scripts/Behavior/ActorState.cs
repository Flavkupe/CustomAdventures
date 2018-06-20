using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorState : State
{
    public abstract IEnumerator TakeAction();

    private TileAI _actor;

    protected TileAI Actor
    {
        get
        {
            Debug.Assert(_actor != null, "Be sure to call Initialize on the ActorState object!");
            return _actor;
        }
    }

    public void Initialize(TileAI actor)
    {
        _actor = actor;
    }
}
