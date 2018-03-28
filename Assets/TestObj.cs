using System.Collections;
using UnityEngine;

public class TestObj : MonoBehaviour {

    public float Speed = 0.2f;

    // Use this for initialization
    private void Start () {
		
	}

    // Update is called once per frame
    private void Update () {
		
	}

    public IEnumerator MoveUp()
    {
        while (transform.position.y < 3.0f)
        {
            transform.position = transform.position.IncrementBy(0, Speed, 0);
            yield return null;
        }
    }

    public IEnumerator MoveDown()
    {
        while (transform.position.y > -4.0f)
        {
            transform.position = transform.position.IncrementBy(0, -Speed, 0);
            yield return null;
        }
    }
}
