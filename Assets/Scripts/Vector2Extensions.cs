using UnityEngine;
using System.Collections;

/// <summary>
/// Extend functionality of Unity Vector2 class
/// </summary>
public static class Vector2Extensions
{
    /// <summary>
    /// Returns perpendicular vector
    /// </summary>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static Vector2 Perpendicular(this Vector2 vector2)
    {
        return new Vector2(-vector2.y, vector2.x);
    }

    /// <summary>
    /// Cheap fix to Unity's coordinate system starting at top left compared to gimp's which starts at bottom left
    /// </summary>
    /// <param name="vector2"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static Vector2 ToUnityCoordSys(this Vector2 vector2, float offset)
    {
        return new Vector2(vector2.x, offset - vector2.y);
    }

}
 