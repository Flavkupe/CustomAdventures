using System;
using UnityEngine;

public abstract class ScriptableState : ScriptableObject
{
    public ScriptableTransition[] Transitions;
}

[Serializable]
public class ScriptableTransition
{
    public ScriptableDecision ScriptableDecision;
    public ScriptableState Next;
}

public abstract class ScriptableDecision : ScriptableObject, IDecision
{
    public bool Negate;

    public bool Evaluate(GameContext context)
    {
        var evaluation = Evaluation(context);
        return Negate ? !evaluation : evaluation;
    }

    protected abstract bool Evaluation(GameContext context);
}