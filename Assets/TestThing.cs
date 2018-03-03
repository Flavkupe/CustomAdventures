using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestThing : MonoBehaviour {



	// Use this for initialization
	void Start () {
        var objects = this.GetComponentsInChildren<TestObj>();
        var routine = Routine.Create(objects[0].MoveUp);
        routine.Then(() => { Destroy(objects[1].gameObject, 2.0f); })
               .Then(Routine.Create(objects[1].MoveUp))
               .Then(Routine.Create(objects[2].MoveUp));

        StartCoroutine(routine);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
