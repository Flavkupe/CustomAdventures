using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Decision", menuName = "Create Behavior/Decisions/Player Is In Range", order = 1)]
public class PlayerIsInRangeDecision : Decision
{
    [Tooltip("Range in which to check, based on RangeType")]
    public int Range;

    [Tooltip("Whether to use the AI's vision stats instead of Range. Overrides Range.")]
    public bool UseVisionForRange;

    public TileRangeType RangeType;

    protected override bool Evaluation(TileAI subject, GameContext context)
    {
        var range = UseVisionForRange ? subject.GetModifiedStats().VisibilityRange : Range;
        return context.Dungeon.Grid.GetEntities(RangeType, subject.XCoord, subject.YCoord, range, TileEntityType.Player).Count > 0;
    }
}

