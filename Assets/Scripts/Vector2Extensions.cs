using UnityEngine;
using System.Collections;

public static class Vector2Extensions
{
    public static Vector2 Perpendicular(this Vector2 vector2)
    {
        return new Vector2(-vector2.y, vector2.x);
    }

}
