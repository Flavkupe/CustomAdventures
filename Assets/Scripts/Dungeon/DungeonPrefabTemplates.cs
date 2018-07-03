using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Template", menuName = "Create Template List/Dungeon Prefab Templates", order = 0)]
public class DungeonPrefabTemplates : ScriptableObject
{
    [Serializable]
    public class EntityPartPrefabs
    {
        public ThoughtBubble ThoughtBubble;
    }

    [Serializable]
    public class DungeonPartPrefabs
    {
        public GridTile GridTile;
    }

    public EntityPartPrefabs EntityParts;

    public DungeonPartPrefabs DungeonParts;
}

