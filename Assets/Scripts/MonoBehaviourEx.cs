using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourEx : MonoBehaviour
{
    public IEnumerator MoveToSpotCoroutine(Vector3 target, float speed)
    {
        float targetDistance = Vector3.Distance(target, this.transform.position);
        float distanceTravelled = 0.0f;
        while (distanceTravelled < targetDistance)
        {
            float delta = Time.deltaTime * speed;
            this.transform.position = Vector3.MoveTowards(this.transform.position, target, delta);
            distanceTravelled += delta;
            yield return null;
        }
    }

    public IEnumerator RotateCoroutine(Vector3 axis, float angle, float speed)
    {
        float totalRot = 0;
        while (totalRot < angle)
        {
            float delta = Time.deltaTime * speed;
            this.transform.Rotate(axis, delta);
            totalRot += delta;
            yield return null;
        }        
    }

    // Use this for initialization
    void Start ()
    {		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
