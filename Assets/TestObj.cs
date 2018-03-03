using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObj : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator MoveUp()
    {
        while (this.transform.position.y < 3.0f)
        {
            this.transform.position = this.transform.position.IncrementBy(0, 0.2f, 0);
            yield return null;
        }
    }
}
