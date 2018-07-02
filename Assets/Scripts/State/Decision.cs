using UnityEngine;

public abstract class Decision : ScriptableObject
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
/// How to interpret the Decision; do all have to be true, or at least one?
/// </summary>
public enum DecisionEvaluationType
{
    AllMustBeTrue,
    AnyCanBeTrue
}