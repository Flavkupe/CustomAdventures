using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourEx : MonoBehaviour
{
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

    public T InstantiateOfType<T>(string name = null) where T : MonoBehaviour
    {
        GameObject obj = new GameObject(name ?? typeof(T).Name);
        T newObj = obj.AddComponent<T>();
        return newObj;
    }

    public T InstantiateOfType<T>(Type type, string name = null) where T : class
    {
        if (!typeof(T).IsAssignableFrom(type))
        {
            Debug.LogError(string.Format("Trying instantiate things of different type as specified! type: {0}, T: {1}", type.FullName, typeof(T).FullName));
            return null;
        }

        GameObject obj = new GameObject(name ?? typeof(T).Name);
        T newObj = obj.AddComponent(type) as T;
        return newObj;
    }

    // Use this for initialization
    void Start ()
    {		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
