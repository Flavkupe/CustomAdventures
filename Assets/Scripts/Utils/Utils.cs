using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class Utils
{
    public static bool HasFlag(int enumValue, int flagValue)
    {
        return (enumValue & flagValue) != 0;
    }

    public static void DoForXY(int dimX, int dimY, Action<int,int> action)
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
        for (int x = startX; x < endX; x++)
        {
            for (int y = startX; y < endY; y++)
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
            case Direction.Right:
                return Direction.Left;            
        }
    }
}

public static class ExtensionFunctions
{
    public static IEnumerator DoNextFrame(this MonoBehaviour obj, Action action)
    {
        yield return null;
        action.Invoke();
    }

    public static T GetRandom<T>(this IList<T> list)
    {
        if (list.Count == 0)
        {
            return default(T);
        }

        return list[UnityEngine.Random.Range(0, list.Count)];
    }
    
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Utils.Rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static IEnumerator MoveToSpotCoroutine(this MonoBehaviour obj, Vector3 target, float speed, bool allowMouseSpeedup = true)
    {
        return obj.gameObject.MoveToSpotCoroutine(target, speed, allowMouseSpeedup);
    }

    public static IEnumerator MoveToSpotCoroutine(this GameObject obj, Vector3 target, float speed, bool allowMouseSpeedup = true)
    {
        return obj.MoveToSpotAndScaleCoroutine(target, speed, 0.0f, allowMouseSpeedup);
    }

    public static IEnumerator MoveToSpotAndScaleCoroutine(this GameObject obj, Vector3 target, float speed, float targetScaleChange, bool allowMouseSpeedup = true)
    {
        Vector3 targetScale = obj.transform.localScale.IncrementBy(targetScaleChange, targetScaleChange, targetScaleChange);
        float targetDistance = Vector3.Distance(target, obj.transform.position);
        float distanceTravelled = 0.0f;
        while (distanceTravelled < targetDistance)
        {
            float speedMultiplier = allowMouseSpeedup ? GameManager.Instance.GetMouseDownSpeedMultiplier() : 1.0f;
            float delta = Time.deltaTime * speed * speedMultiplier;
            float proportion = delta / targetDistance;
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, target, delta);

            if (targetScaleChange != 0.0f)
            {
                float scaleChange = proportion * targetScaleChange;
                Vector3 newScale = obj.transform.localScale.IncrementBy(scaleChange, scaleChange, scaleChange);
                obj.transform.localScale = newScale;
            }

            distanceTravelled += delta;
            yield return null;
        }

        obj.transform.localScale = targetScale;
    }

    public static Vector3 OffsetBy(this Vector3 vector, float offset)
    {
        return vector.OffsetBy(offset, offset);
    }

    public static Vector3 OffsetBy(this Vector3 vector, float xOffset, float yOffset)
    {
        return new Vector3(vector.x - xOffset, vector.y - yOffset);
    }

    public static Vector3 IncrementBy(this Vector3 vector, float xOffset, float yOffset, float zOffset)
    {
        return new Vector3(vector.x + xOffset, vector.y + yOffset, vector.z + zOffset);
    }

    #region 2D array extensions    

    public static void ShiftInDirection<T>(this T obj, Direction direction) where T : IHasCoords
    {
        switch (direction)
        {
            case Direction.Up:
                obj.YCoord++;
                break;
            case Direction.Down:
                obj.YCoord--;
                break;
            case Direction.Left:
                obj.XCoord--;
                break;
            case Direction.Right:
                obj.XCoord++;
                break;
            default:
                break;
        }
    }

    public static T GetAdjacent<T>(this T[,] grid, int x, int y, Direction direction) where T : class
    {
        switch (direction)
        {
            case Direction.Up:
                y++;
                break;
            case Direction.Down:
                y--;
                break;
            case Direction.Left:
                x--;
                break;
            case Direction.Right:
                x++;
                break;
        }

        if (grid.IsOffBounds(x, y))
        {
            // Off bounds
            return null;
        }

        return grid[x, y];
    }

    public static bool IsAdjacentOffBoundsOrFull<T>(this T[,] grid, int x, int y, Direction direction) where T : class
    {
        switch (direction)
        {
            case Direction.Up:
                y++;
                break;
            case Direction.Down:
                y--;
                break;
            case Direction.Left:
                x--;
                break;
            case Direction.Right:
                x++;
                break;
        }

        if (grid.IsOffBounds(x, y))
        {
            // Off bounds
            return true;
        }

        return grid[x, y] != null;
    }

    public static bool IsOffBounds<T>(this T[,] grid, int x, int y)
    {
        return x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1);
    }    

    #endregion

    #region UnityEngine.UI Extensions

    /// <summary>
    /// Gets the text of the selected item from the UI Dropdown. Returns emptystring if
    /// the dropdown is empty.
    /// </summary>
    public static string GetSelectedText(this Dropdown dropdown)
    {
        return dropdown.options.Count > dropdown.value ? dropdown.options[dropdown.value].text : string.Empty;
    }

    #endregion

    #region UnityEngine.Graphic Extensions

    public static void SetOpacity(this Graphic graphic, float opacity)
    {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, opacity);
    }

    #endregion

    public static void SwapWith(this Transform transform, Transform other, bool keepYConstant = false)
    {
        Vector3 temp = transform.position;
        if (keepYConstant)
        {
            transform.position = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);
            other.position = new Vector3(temp.x, other.position.y, temp.z);
        }
        else
        {
            transform.position = other.position;
            other.position = temp;
        }
    }
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
}

public class EnumFlagsAttribute : PropertyAttribute { }

public class SingletonObject<T> : MonoBehaviourEx where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
        protected set { instance = value; }
    }
}

/// <summary>
/// A timer that can be used to track cooldowns and 
/// durations of things.
/// </summary>
public class CooldownTimer
{
    /// <summary>
    /// The goal of the timer. Once this time is reached,
    /// the timer is "expired" (ie goes "ding!").
    /// </summary>
    private float baseLine = 0.0f;

    private float currentTime = 0.0f;

    public event EventHandler OnCooldownExpired;

    /// <summary>
    /// Ctor for creating an inactive timer. Set new baseline with
    /// SetBaseline method.
    /// </summary>
    public CooldownTimer()
    {
        this.baseLine = 0.0f;
    }

    /// <summary>
    /// Ctor for creating an active timer with timeout
    /// occurring after time reaches 'baseLine'.
    /// </summary>
    /// <param name="baseLine">How long the timer must tick 
    /// before cooldown expires</param>
    public CooldownTimer(float baseLine)
    {
        this.baseLine = baseLine;
    }

    /// <summary>
    /// Ticks the timer by some amount. Usually goes
    /// in the Update loop using Time.deltaTime, but
    /// delta can be anything. If delta is null, 
    /// Time.deltaTime is used. Returns a reference to
    /// this object, so you can do stuff like
    /// if (timer.Tick().IsExpired) and other such things.
    /// 
    /// Only ticks if the cooldown is active (not expired and
    /// non-zero baseLine). Immediately after expiry, the
    /// OnCooldownExpired event is fired.
    /// </summary>
    /// <param name="delta"></param>
    /// <returns></returns>
    public CooldownTimer Tick(float? delta = null)
    {
        if (this.IsActive)
        {
            this.currentTime += delta ?? Time.deltaTime;
            if (this.IsExpired)
            {
                // On the exact tick after expiry, this event fires.
                if (this.OnCooldownExpired != null)
                {
                    this.OnCooldownExpired(this, new EventArgs());
                }
            }
        }

        return this;
    }

    /// <summary>
    /// Whether or not the timer reached its goal
    /// </summary>
    public bool IsExpired { get { return this.currentTime >= baseLine; } }

    /// <summary>
    /// Whether or not this timer should tick.
    /// </summary>
    public bool IsActive { get { return this.baseLine > 0.0f && !this.IsExpired; } }

    /// <summary>
    /// Resets the timer to be used again.
    /// </summary>
    public void Reset()
    {
        this.currentTime = 0.0f;
    }

    /// <summary>
    /// Changes the target time for the timer.
    /// </summary>
    /// <param name="baseLine"></param>
    public void SetBaseline(float baseLine)
    {
        this.baseLine = baseLine;
    }

}