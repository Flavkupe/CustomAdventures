using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IDungeonStateProvider : IActionDeterminant<DungeonActionType>
{
}

/// <summary>
/// A facade class for the dungeon state controller; provides simplified
/// access to DungeonStateController info
/// </summary>
public class DungeonStateProvider : IDungeonStateProvider
{
    private DungeonStateController _dungeonStateController;
    private AnimationStateController _animationStateController;

    private readonly List<IActionDeterminant<DungeonActionType>> _actionDeterminants = new List<IActionDeterminant<DungeonActionType>>();

    public DungeonStateProvider(DungeonStateController dungeonStateController, AnimationStateController animationStateController)
    {
        _dungeonStateController = dungeonStateController;
        _animationStateController = animationStateController;
        _actionDeterminants.Add(_dungeonStateController);
        _actionDeterminants.Add(_animationStateController);
    }

    public bool CanPerformAction(DungeonActionType actionType)
    {
        return _actionDeterminants.All(a => a.CanPerformAction(actionType));
    }
}

