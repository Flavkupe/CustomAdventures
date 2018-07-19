using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision", menuName = "Create Behavior/Decisions/Composite", order = 0)]
public class CompositeScriptableDecision : ScriptableAIDecision
{
    public ScriptableAIDecision[] ScriptableDecisions;

    public DecisionEvaluationType DecisionEvaluation;

    protected override bool Evaluation(TileAI subject, GameContext context)
    {
        var evaluation = false;
        switch(DecisionEvaluation)
        {
            case DecisionEvaluationType.AllMustBeTrue:
                evaluation = ScriptableDecisions.All(a => a.Evaluate(subject, context));
                break;
            default:
                evaluation = ScriptableDecisions.Any(a => a.Evaluate(subject, context));
                break;
        }

        return Negate ? !evaluation : evaluation;
    }
}
