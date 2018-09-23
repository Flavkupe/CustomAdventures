using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Strategy", menuName = "Create Behavior/Strategy", order = 1)]
public class ActorStrategy : ScriptableObject
{
    [Serializable]
    public class AbilityCountdownOptions
    {
        [Tooltip("Whether the countdown must run before the first use of this ability.")]
        public bool FirstUseRequiresCountdown;

        [Tooltip("How many turns must pass for this ability to be used. If 0, no countdown will be used.")]
        public int Countdown = 0;
    }

    private int _countdown = 0;

    [Tooltip("Options associated with using countdowns.")]
    public AbilityCountdownOptions CountdownOptions;

    [Tooltip("What has to happen in order for this strategy to be picked. If all resolve to false, will try next strategy.")]
    public ScriptableAIDecision[] ScriptableAiDecisions;

    [Tooltip("At the start of each turn, if any of these are true, will revert to base stat. For example, player goes off-sight.")]
    public ScriptableAIDecision[] ResetOn;

    [Tooltip("List of sequential actions to use each turn, if on this state.")]
    public ActorAction[] Actions;

    [Tooltip("Whether or not a free move can be used for this strategy. For example, for walking.")]
    public bool CanUseFreeMove = false;

    [Tooltip("How to interpret the ScriptableAIDecision; do all have to be true, or at least one?")]
    public DecisionEvaluationType DecisionEvaluation;

    public bool CanTakeStrategy(TileAI subject)
    {
        var stats = subject.GetModifiedStats();
        return stats.FullActions > 0 || CanUseFreeMove && stats.FreeMoves > 0;
    }

    public virtual bool Decide(TileAI subject, GameContext context)
    {
        switch (DecisionEvaluation)
        {
            case DecisionEvaluationType.AllMustBeTrue:
                return ScriptableAiDecisions.All(a => a.Evaluate(subject, context));
            default:
                return ScriptableAiDecisions.Any(a => a.Evaluate(subject, context));
        }
    }

    public virtual bool ShouldAbandon(TileAI subject, GameContext context)
    {
        return ResetOn != null && ResetOn.Any(a => a.Evaluate(subject, context));
    }

    public virtual void EnterStrategy(TileAI subject)
    {
        // Initialize countdown if applicable
        if (CountdownOptions != null)
        {
            _countdown = CountdownOptions.FirstUseRequiresCountdown ? CountdownOptions.Countdown : 0;
        }

        subject.HideThoughtBubble();
    }

    public virtual void ExitStrategy(TileAI subject)
    {
        // Reset countdown, even if not used.
        _countdown = 0;
        subject.HideThoughtBubble();
    }

    public IEnumerator Execute(TileAI subject, GameContext context)
    {
        Debug.Assert(CanTakeStrategy(subject), "Trying to perform action without free moves!");

        // Tally action points and free moves first
        if (CanUseFreeMove && subject.GetModifiedStats().FreeMoves > 0)
        {
            // First use free moves if available
            subject.CurrentStats.FreeMoves--;
        }
        else
        {
            // If can't use free move, use full actions and forgo free moves for turn
            subject.CurrentStats.FreeMoves.Value = 0;
            subject.CurrentStats.FullActions--;
        }

        // Handle countdown actions and toggle subject's Thought bubble
        if (_countdown > 0)
        {            
            subject.SetThoughtBubbleText(_countdown.ToString());
            _countdown--;
            yield break;
        }
        else
        {
            yield return PerformActions(subject, context);
            subject.HideThoughtBubble();
            _countdown = CountdownOptions?.Countdown ?? 0;
        }        
    }

    protected virtual IEnumerator PerformActions(TileAI subject, GameContext context)
    {
        foreach (var action in Actions)
        {
            yield return action.PerformAction(subject, context);
        }      
    }
}

