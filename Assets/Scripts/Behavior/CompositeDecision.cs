using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision", menuName = "Create Behavior/Decisions/Composite", order = 0)]
public class CompositeDecision : Decision
{
    public Decision[] Decisions;

    public DecisionEvaluationType DecisionEvaluation;

    protected override bool Evaluation(TileAI subject, GameContext context)
    {
        var evaluation = false;
        switch(DecisionEvaluation)
        {
            case DecisionEvaluationType.AllMustBeTrue:
                evaluation = Decisions.All(a => a.Evaluate(subject, context));
                break;
            default:
                evaluation = Decisions.Any(a => a.Evaluate(subject, context));
                break;
        }

        return Negate ? !evaluation : evaluation;
    }
}
