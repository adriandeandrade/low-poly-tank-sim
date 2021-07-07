using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    /// <summary>
    /// Reflects v across the plane defined by normal n.
    /// </summary>
    public static Vector3 Reflect(
            this Vector3 v,
            Vector3 n)
    {
        var projection = Vector3.ProjectOnPlane(v, n);
        return 2f * projection - v;
    }

    /// <summary>
    /// Returns the vector from this to another position vector.
    /// </summary>
    public static Vector3 To(this Vector3 v, Vector3 other)
    {
        return other - v;
    }
}
