using System;
using System.Collections;
using UnityEngine;

public class MonoBehaviourEx : MonoBehaviour
{
    public virtual IEnumerator TwitchTowards(Vector3 target, float speed = 5.0f)
    {
        var targetDirection = transform.position.GetRelativeDirection(target);
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

    public T InstantiateOfType<T>(string objName = null) where T : MonoBehaviour
    {
        return Utils.InstantiateOfType<T>(objName);
    }

    public T InstantiateOfType<T>(Type type, string objName = null) where T : class
    {
        if (!typeof(T).IsAssignableFrom(type))
        {
            Debug.LogError($"Trying instantiate things of different type as specified! type: {type.FullName}, T: {typeof(T).FullName}");
            return null;
        }

        var obj = new GameObject(objName ?? typeof(T).Name);
        var newObj = obj.AddComponent(type) as T;
        return newObj;
    }

    protected void StartRoutine(IEnumerator routine)
    {
        StartRoutine(Routine.Create(routine));
    }

    protected void StartRoutine(Routine routine)
    {
        if (routine == null)
        {
            return;
        }

        routine.Catch(OnRoutineException);
        StartCoroutine(routine);
    }

    protected virtual void OnRoutineException(Exception ex)
    {
    }
}
