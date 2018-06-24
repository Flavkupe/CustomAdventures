using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PrefabManager : SingletonObject<PrefabManager>
{
    [Serializable]
    public class EntityPartPrefabs
    {
        public ThoughtBubble ThoughtBubble;
    }

    public EntityPartPrefabs EntityParts;

    private void Awake()
    {
        Instance = this;
    }
}

