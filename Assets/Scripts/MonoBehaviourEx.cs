using System;
using System.Collections;
using UnityEngine;

public class MonoBehaviourEx : MonoBehaviour
{
    public virtual IEnumerator TwitchTowards(Vector3 target, float speed = 5.0f)
    {
        Direction targetDirection = transform.position.GetRelativeDirection(target);
        yield return TwitchTowards(targetDirection, speed);
    }

    public virtual IEnumerator TwitchTowards(Direction direction, float speed = 5.0f)
    {
        Vector3 directionVector = new Vector3();
        switch (direction)
        {
            case Direction.Down:
                directionVector = Vector3.down;
                break;
            case Direction.Left:
                directionVector = Vector3.left;
                break;
            case Direction.Right:
                directionVector = Vector3.right;
                break;
            case Direction.Up:
                directionVector = Vector3.up;
                break;
        }

        directionVector *= 0.2f;
        Vector3 target = transform.position + directionVector;
        Vector3 start = transform.position;
        yield return transform.MoveToSpotCoroutine(target, speed, false);
        yield return transform.MoveToSpotCoroutine(start, speed, false);
    }

    public IEnumerator RotateCoroutine(Vector3 axis, float angle, float speed)
    {
        float totalRot = 0;
        while (totalRot < angle)
        {
            float speedMultiplier = Game.States.GetMouseDownSpeedMultiplier();
            float delta = Time.deltaTime * speed * speedMultiplier;
            transform.Rotate(axis, delta);
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
    private void Start ()
    {		
	}

    // Update is called once per frame
    private void Update () {
		
	}
}
