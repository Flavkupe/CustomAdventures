using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [CreateAssetMenu(fileName = "Template", menuName = "Create Template List/UI Prefab Templates", order = 0)]
    public class UIPrefabTemplates : ScriptableObject
    {
        public UIPartPrefabs UIParts;

        [Serializable]
        public class UIPartPrefabs
        {
            public UITooltip HoverTooltip;
        }
    }
}
