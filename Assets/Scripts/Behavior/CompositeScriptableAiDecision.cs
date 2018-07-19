using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision", menuName = "Create Behavior/Decisions/Composite", order = 0)]
public class CompositeScriptableAiDecision : ScriptableAIDecision
{
    public ScriptableAIDecision[] ScriptableAiDecisions;

    public DecisionEvaluationType DecisionEvaluation;

    protected override bool Evaluation(TileAI subject, GameContext context)
    {
        var evaluation = false;
        switch(DecisionEvaluation)
        {
            case DecisionEvaluationType.AllMustBeTrue:
                evaluation = ScriptableAiDecisions.All(a => a.Evaluate(subject, context));
                break;
            default:
                evaluation = ScriptableAiDecisions.Any(a => a.Evaluate(subject, context));
                break;
        }

        return Negate ? !evaluation : evaluation;
    }
}
