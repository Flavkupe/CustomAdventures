using UnityEngine;

public class TestThing : MonoBehaviour {



    // Use this for initialization
    private void Start () {
        MoveAllParallelRoutine();
    }

    private void MoveAllParallelRoutine()
    {
        var objects = GetComponentsInChildren<TestObj>();
        var routine1 = Routine.Create(objects[0].MoveUp);
        var routine2 = Routine.Create(objects[1].MoveUp);
        var routine3 = Routine.Create(objects[2].MoveUp);
        var set = new ParallelRoutineSet(routine1, routine2, routine3);
        var setRoutine = set.AsRoutine();
        setRoutine.Then(objects[3].MoveDown)
                  .Then(objects[4].MoveDown);

        StartCoroutine(setRoutine);
    }

    private void MoveAllRoutine()
    {
        var objects = GetComponentsInChildren<TestObj>();
        var routine = Routine.Create(objects[0].MoveUp);
        routine.Then(() => { Destroy(objects[1].gameObject, 2.0f); })
               .Then(Routine.Create(objects[1].MoveUp))
               .Then(Routine.Create(objects[2].MoveUp));

        StartCoroutine(routine);
    }

    // Update is called once per frame
    private void Update () {
		
	}
}
