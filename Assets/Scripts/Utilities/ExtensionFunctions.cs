﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public static class ExtensionFunctions
{
    /// <summary>
    /// Gets whether target is to the left, right, top or bottom
    /// of this object. Picks relative direction of highest maginitude
    /// </summary>
    public static Direction GetRelativeDirection(this Vector3 me, Vector3 target)
    {
        float x = target.x - me.x;
        float y = target.y - me.y;
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            return x >= 0 ? Direction.Right : Direction.Left;
        }

        return y >= 0 ? Direction.Up : Direction.Down;
    }

    public static IEnumerator DoNextFrame(this MonoBehaviour obj, Action action)
    {
        yield return null;
        action.Invoke();
    }

    public static T GetMax<T, R>(this IList<T> list, Func<T, R> selector) where R : IComparable<R>
    {
        if (list.Count == 0)
        {
            return default(T);
        }

        T max = list[0];
        foreach (var item in list)
        {
            if (selector(item).CompareTo(selector(max)) > 0)
            {
                max = item;
            }
        }

        return max;
    }

    public static T GetRandom<T>(this IList<T> list)
    {
        if (list.Count == 0)
        {
            return default(T);
        }

        if (list.Count == 1)
        {
            return list[0];
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

    public static void SetOrIncrement<T>(this Dictionary<T, int> dict, T key)
    {
        if (dict.ContainsKey(key))
        {
            dict[key]++;
        }
        else
        {
            dict[key] = 1;
        }
    }

    public static IEnumerator MoveToSpotCoroutine(this Transform obj, Vector3 target, float speed, bool allowMouseSpeedup = true)
    {
        return obj.MoveToSpotAndScaleCoroutine(target, speed, 0.0f, allowMouseSpeedup);
    }

    public static IEnumerator MoveToSpotAndScaleCoroutine(this Transform obj, Vector3 target, float speed,
        float targetScaleChange, bool allowMouseSpeedup = true)
    {
        return obj.MoveToSpotCoroutine(target,
            new MoveToSpotOptions
            {
                Speed = speed,
                ScaleChange = targetScaleChange,
                AllowMouseSpeedup = allowMouseSpeedup
            });
    }

    public static IEnumerator MoveToSpotCoroutine(this Transform obj, Vector3 target, MoveToSpotOptions options)
    {
        Vector3 targetScale = obj.localScale.IncrementBy(options.ScaleChange, options.ScaleChange, options.ScaleChange);
        float targetDistance = Vector3.Distance(target, obj.position);
        float distanceTravelled = 0.0f;
        while (distanceTravelled < targetDistance)
        {
            float speedMultiplier = options.AllowMouseSpeedup ? Utils.GetMouseDownSpeedMultiplier() : 1.0f;
            float delta = Time.deltaTime * options.Speed * speedMultiplier;
            float proportion = delta / targetDistance;
            obj.position = Vector3.MoveTowards(obj.position, target, delta);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (options.ScaleChange != 0.0f)
            {
                float scaleChange = proportion * options.ScaleChange;
                Vector3 newScale = obj.localScale.IncrementBy(scaleChange, scaleChange, scaleChange);
                obj.localScale = newScale;
            }

            if (options.RotationChange != 0.0f)
            {
                obj.Rotate(Vector3.back, options.RotationChange * delta);
            }

            distanceTravelled += delta;
            yield return null;
        }

        obj.localScale = targetScale;
    }

    public static Vector3 OffsetBy(this Vector3 vector, float offset)
    {
        return vector.OffsetBy(offset, offset);
    }

    public static Vector3 OffsetBy(this Vector3 vector, float xOffset, float yOffset, float zOffset = 0.0f)
    {
        return new Vector3(vector.x - xOffset, vector.y - yOffset, vector.z - zOffset);
    }

    public static Vector3 SetX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y, vector.z);
    }

    public static Vector3 SetY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }

    public static Vector3 SetZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }

    public static Vector3 IncrementBy(this Vector3 vector, float xOffset, float yOffset, float zOffset)
    {
        return new Vector3(vector.x + xOffset, vector.y + yOffset, vector.z + zOffset);
    }

    public static void SetParentAndPos(this Transform transform, Transform parent)
    {
        transform.SetParent(parent);
        transform.position = parent.position;
    }

    public static IEnumerator RotateCoroutine(this Transform transform, Vector3 axis, float angle, float speed)
    {
        float totalRot = 0;
        while (totalRot < angle)
        {
            float speedMultiplier = Utils.GetMouseDownSpeedMultiplier();
            float delta = Time.deltaTime * speed * speedMultiplier;
            transform.Rotate(axis, delta);
            totalRot += delta;
            yield return null;
        }
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
