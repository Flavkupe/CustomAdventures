using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.State;
using UnityEngine;

public class World : SingletonObject<World>
{
    private GlobalUpdateController _globalUpdater;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start ()
	{
	    var allObjects = FindObjectsOfType<Object>();
        _globalUpdater = Utils.InstantiateOfType<GlobalUpdateController>();
	    _globalUpdater.InitObjects(allObjects);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SimpleEventHappened(SimpleWorldEvent eventType)
    {
        if (_globalUpdater != null)
        {
            _globalUpdater.SimpleEventHappened(eventType);
        }
    }
}
