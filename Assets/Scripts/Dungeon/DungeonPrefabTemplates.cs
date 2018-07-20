﻿using System;
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

    [Serializable]
    public class CardPartPrefabs
    {
        [Serializable]
        public class AnimationEffects
        {
            public AnimationEffectData DefaultCardTriggerEffect;
            public AnimationEffectData DefaultCardMoveToEffect;
        }

        public AnimationEffects Effects;
    }

    public CardPartPrefabs CardParts;

    public EntityPartPrefabs EntityParts;

    public DungeonPartPrefabs DungeonParts;
}
