using Assets.Scripts.UI.State;
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
    private PlayerStateController _playerStateController;
    private UIStateController _uiStateController;

    private IEnumerable<IStateController> _controllers;
    private readonly List<IActionDeterminant<DungeonActionType>> _actionDeterminants = new List<IActionDeterminant<DungeonActionType>>();

    private Dungeon _dungeon;

    public DungeonStateProvider(Dungeon dungeon,
        DungeonStateController dungeonStateController, 
        AnimationStateController animationStateController,
        PlayerStateController playerStateController,
        UIStateController uiStateController)
    {
        _dungeon = dungeon;
        _dungeonStateController = dungeonStateController;
        _animationStateController = animationStateController;
        _playerStateController = playerStateController;
        _uiStateController = uiStateController;

        _controllers = new List<IStateController>(new IStateController[] {
            // Add all controllers
            _dungeonStateController,
            _animationStateController,
            _playerStateController,
            _uiStateController
        });

        // Add all controllers that determine dungeon actions
        _actionDeterminants.AddRange(_controllers.OfType<IActionDeterminant<DungeonActionType>>());
    }

    public IEnumerable<IStateController> Controllers => _controllers;

    public bool CanPerformAction(DungeonActionType actionType)
    {
        return _actionDeterminants.All(a => a.CanPerformAction(actionType));
    }

    public void HandleNewEvent<TEventType>(TEventType eventType) where TEventType : struct
    {
        foreach (var controller in  _controllers)
        {
            var stateController = controller as IStateController<TEventType>;
            if (stateController != null)
            {
                // Will handle the event using default behavior
                stateController.HandleNewEvent(eventType, _dungeon.GetGameContext());
            }
            else if (controller != null)
            {
                // Will try to handle the event if the state supports it
                controller.HandleNewEvent(eventType, _dungeon.GetGameContext());
            }
        }
    }

    private List<DungeonEventType> DungeonEventList = new List<DungeonEventType>();

    public void RegisterToBroadcastEvents<TEventType>(EventHandler<TEventType> callback) where TEventType : struct
    {
        foreach (var controller in _controllers)
        {
            var stateController = controller as IStateController<TEventType>;
            if (stateController != null)
            {
                stateController.NewEventRaised += callback;
            }
        }
    }

    public void UnregisterFromBroadcastEvents<TEventType>(EventHandler<TEventType> callback) where TEventType : struct
    {
        foreach (var controller in _controllers)
        {
            var stateController = controller as IStateController<TEventType>;
            if (stateController != null)
            {
                stateController.NewEventRaised -= callback;
            }
        }
    }
}
