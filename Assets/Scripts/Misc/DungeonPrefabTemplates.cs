using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Template", menuName = "Create Template List/Dungeon Prefab Templates", order = 0)]
public class DungeonPrefabTemplates : ScriptableObject
{
    [Serializable]
    public class EntityPartPrefabs
    {
        public ThoughtBubble ThoughtBubble;
    }

    public EntityPartPrefabs EntityParts;
}

