using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Strategy", menuName = "Create Behavior/Strategy", order = 1)]
public class ActorStrategy : ScriptableObject
{
    [Tooltip("What has to happen in order for this strategy to be picked. If all resolve to false, will try next strategy.")]
    public Decision[] Decisions;

    [Tooltip("At the start of each turn, if any of these are true, will revert to base stat. For example, player goes off-sight.")]
    public Decision ResetOn;

    [Tooltip("List of sequential actions to use each turn, if on this state.")]
    public ActorAction[] Actions;

    [Tooltip("Whether or not a free move can be used for this strategy. For example, for walking.")]
    public bool CanUseFreeMove = false;

    [Tooltip("How to interpret the Decision; do all have to be true, or at least one?")]
    public DecisionEvaluationType DecisionEvaluation;

    public bool CanTakeStrategy(TileAI subject)
    {
        return subject.CurrentStats.FullActions > 0 || CanUseFreeMove && subject.CurrentStats.FreeMoves > 0;
    }

    public virtual bool Decide(TileAI subject, GameContext context)
    {
        switch (DecisionEvaluation)
        {
            case DecisionEvaluationType.AllMustBeTrue:
                return Decisions.All(a => a.Evaluate(subject, context));
            default:
                return Decisions.Any(a => a.Evaluate(subject, context));
        }
    }

    public virtual bool ShouldAbandon(TileAI subject, GameContext context)
    {
        return ResetOn != null && ResetOn.Evaluate(subject, context);
    }

    public virtual IEnumerator PerformActions(TileAI subject, GameContext context)
    {
        Debug.Assert(CanTakeStrategy(subject), "Trying to perform action without free moves!");

        foreach (var action in Actions)
        {
            yield return action.PerformAction(subject, context);
        }

        if (CanUseFreeMove && subject.CurrentStats.FreeMoves > 0)
        {
            // First use free moves if available
            subject.CurrentStats.FreeMoves--;
        }
        else
        {
            // If can't use free move, use full actions and forgo free moves for turn
            subject.CurrentStats.FreeMoves = 0;
            subject.CurrentStats.FullActions--;
        }
    }

    /// <summary>
    /// How to interpret the Decision; do all have to be true, or at least one?
    /// </summary>
    public enum DecisionEvaluationType
    {
        AllMustBeTrue,
        AnyCanBeTrue
    }
}

