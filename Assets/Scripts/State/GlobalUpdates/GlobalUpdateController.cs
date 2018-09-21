using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.State
{
    public class GlobalUpdateController : MonoBehaviour
    {
        private List<IUpdatesWhen> _updateable;

        public void InitObjects(Object[] objects)
        {
            _updateable = objects.OfType<IUpdatesWhen>().ToList();
        }

        public void SimpleEventHappened(SimpleWorldEvent eventType)
        {
            foreach (var obj in _updateable.Where(a => a.RequiredCondition == eventType))
            {
                obj.Updated();
            }
        }
    }
}
