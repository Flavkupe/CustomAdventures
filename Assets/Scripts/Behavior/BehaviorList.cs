using UnityEngine;

[CreateAssetMenu(fileName = "BehaviorList", menuName = "Create Behavior/Behavior List", order = 1)]
public class BehaviorList : ScriptableObject
{
    public ActorStrategy[] Strategies;
}
