using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.State;
using UnityEngine;

public class World : SingletonObject<World>
{
    private List<IUpdatesWhen> _updateable;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start ()
	{
	    var allObjects = FindObjectsOfType<Object>();
	    InitObjects(allObjects);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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
