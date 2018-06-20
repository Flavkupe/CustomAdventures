using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ActorStrategy : ScriptableObject
{
    [Tooltip("What has to happen in order for this strategy to be picked.")]
    public Decision Decision;

    public ActorStrategy Next;

    protected virtual bool Decide(GameContext context)
    {
        return Decision.Evaluate(context);
    }

    private TileAI _actor;

    protected TileAI Actor
    {
        get
        {
            Debug.Assert(_actor != null, "Be sure to call Initialize on the ActorStrategy object!");
            return _actor;
        }
    }

    public void Initialize(TileAI actor)
    {
        _actor = actor;
    }
}

