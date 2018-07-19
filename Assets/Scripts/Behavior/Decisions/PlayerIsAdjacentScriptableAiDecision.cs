using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableAIDecision", menuName = "Create Behavior/ScriptableAiDecisions/Player Is Adjacent", order = 1)]
public class PlayerIsAdjacentScriptableAiDecision : ScriptableAIDecision
{
    protected override bool Evaluation(TileAI subject, GameContext context)
    {
        return context.Dungeon.Grid.GetNeighbors(subject.XCoord, subject.YCoord)
            .Where(t => t != null && t.GetTileEntity() != null)
            .Select(a => a.GetTileEntity()).Any(b => b == context.Player);
    }
}

