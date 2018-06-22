using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Built-in strategy that does nothing
/// </summary>
public class IdleStrategy : ActorStrategy
{
    private IdleAction _idleAction;
    private IdleAction IdleAction
    {
        get
        {
            if (_idleAction == null)
            {
                _idleAction = CreateInstance<IdleAction>();
            }

            return _idleAction;
        }
    }
    public override bool Decide(TileAI subject, GameContext context)
    {
        // Idle strategy is always picked if possible
        return Decisions == null || Decisions.Length == 0 || base.Decide(subject, context);
    }

    public override bool ShouldAbandon(TileAI subject, GameContext context)
    {
        // Unless otherwise stated, will always abandon this
        return ResetOn == null || base.ShouldAbandon(subject, context);
    }

    public override IEnumerator PerformActions(TileAI subject, GameContext context)
    {
        // use up all actions
        subject.CurrentStats.FreeMoves = 0;
        subject.CurrentStats.FullActions = 0;
        yield return IdleAction;
    }
}
