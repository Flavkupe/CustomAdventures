using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BehaviorList", menuName = "Create Behavior/Behavior List", order = 1)]
public class BehaviorList : ScriptableObject
{
    public ActorStrategy[] Strategies;
}
