﻿using UnityEngine;

public class VectorUtil {

    public static Vector2 RotateVector(Vector2 vector, float radians)
    {
        return new Vector2(vector.x * Mathf.Cos(radians) - vector.y * Mathf.Sin(radians),
            vector.x * Mathf.Sin(radians) + vector.y * Mathf.Cos(radians));
    }

    public static Vector3 RotateVector(Vector3 vector, float radians)
    {
        return new Vector3(vector.x * Mathf.Cos(radians) - vector.y * Mathf.Sin(radians),
            vector.x * Mathf.Sin(radians) + vector.y * Mathf.Cos(radians), 0);
    }

    // Calculates normalized vector from transform to mouse pointer.
    public static Vector2 DirectionToMousePointer(Transform transform)
    {
        Vector3 mouse = Input.mousePosition;
        // WorldToScreenPoint converts a point from world space into screen space (defined in pixels, bottom-left of screen is (0,0)).
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        return new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y).normalized;
    }

    public static float DistanceToMousePointer(Transform transform)
    {
        // Must convert to Vector2 - otherwise distance is made incorrect by the Z-axis.
        return ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).magnitude;
    }

    public static float AngleToMousePointer(Transform transform)
    {
        Vector2 offset = VectorUtil.DirectionToMousePointer(transform);
        return Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
    }
}
