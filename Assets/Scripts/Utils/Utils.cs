using System;
using UnityEngine;

public static class Utils
{
    public static bool HasFlag(int enumValue, int flagValue)
    {
        return (enumValue & flagValue) != 0;
    }

    public static void DoForXY(int dimX, int dimY, Action<int, int> action)
    {
        for (int x = 0; x < dimX; x++)
        {
            for (int y = 0; y < dimY; y++)
            {
                action(x, y);
            }
        }
    }

    public static void DoFromXYToXY(int startX, int startY, int endX, int endY, Action<int, int> action)
    {
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                action(x, y);
            }
        }
    }

    public static System.Random Rand = new System.Random();

    public static Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            default:
                return Direction.Left;
        }
    }

    public static T Clamp<T>(T min, T max, T value) where T : IComparable
    {
        if (value.CompareTo(min) < 0) return min;
        if (value.CompareTo(max) > 0) return max;
        return value;
    }

    public static T InstantiateOfType<T>(string name = null) where T : MonoBehaviour
    {
        GameObject obj = new GameObject(name ?? typeof(T).Name);
        T newObj = obj.AddComponent<T>();
        return newObj;
    }

    public static T InstantiateOfType<T>(Type type, string objName = null) where T : class
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

    public static float GetMouseDownSpeedMultiplier()
    {
        return Input.GetMouseButton(0) ? 3.0f : 1.0f;
    }

    public static Vector3 GetWorldMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}

public class MoveToSpotOptions
{
    public bool AllowMouseSpeedup = true;

    public float ScaleChange = 0.0f;

    public float Speed = 10.0f;

    public float RotationChange = 0.0f;
}

public static class OtherUtils
{
    /// <summary>
    /// Determine the signed angle between two vectors, with normal 'n'
    /// as the rotation axis.
    /// </summary>
    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    public static float RoundOffTo(float val, float toWhat)
    {
        return Mathf.Round(val / toWhat) * toWhat;
    }

    public static Vector3 RoundOffTo(Vector3 vector, float toWhat)
    {
        float x = Mathf.Round(vector.x / toWhat) * toWhat;
        float y = Mathf.Round(vector.y / toWhat) * toWhat;
        float z = Mathf.Round(vector.y / toWhat) * toWhat;
        return new Vector3(x, y, z);
    }
}

public static class EnumUtils
{
    /// <summary>
    /// Parse an enum string into its value. Asserts if this fails.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="valueName">The name of the enum, as a string.</param>
    /// <returns>The enum value if valid, the default enum value otherwise.</returns>
    public static T ParseEnumOrDefault<T>(string valueName) where T : struct
    {
        try
        {
            T enumValue = (T)Enum.Parse(typeof(T), valueName);
            return enumValue;
        }
        catch
        {
            Debug.Assert(false, string.Format("Unkown enum value {0}", valueName));
        }

        return default(T);
    }

    public static void DoForeachEnumValue<T>(Action<T> action) where T : struct
    {
        foreach (T item in Enum.GetValues(typeof(T)))
        {
            action(item);
        }
    }
}

public class EnumFlagsAttribute : PropertyAttribute { }

public class SingletonObject<T> : MonoBehaviourEx where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get { return _instance; }
        protected set { _instance = value; }
    }
}

public class Singleton<T> where T : new()
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }

            return _instance;
        }
    }
}