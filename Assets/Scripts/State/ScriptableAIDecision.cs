using UnityEngine;

public interface IAIDecision
{
    bool Evaluate(TileAI subject, GameContext context);
}

public abstract class ScriptableAIDecision : ScriptableObject, IAIDecision
{
    public bool Negate;

    public bool Evaluate(TileAI subject, GameContext context)
    {
        var evaluation = Evaluation(subject, context);
        return Negate ? !evaluation : evaluation;
    }

    protected abstract bool Evaluation(TileAI subject, GameContext context);
}

/// <summary>
/// How to interpret the ScriptableAIDecision; do all have to be true, or at least one?
/// </summary>
public enum DecisionEvaluationType
{
    AllMustBeTrue,
    AnyCanBeTrue
}