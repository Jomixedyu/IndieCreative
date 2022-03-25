using System;
using UnityEngine;

public static class Detector2D
{
    public static bool Sector(Vector2 point, Vector2 target, Vector2 dir, float radius, float degree)
    {
        Vector2 apos = point;
        Vector2 bpos = target;
        if (Vector2.Distance(apos, bpos) > radius)
        {
            return false;
        }
        if (Vector2.Angle(dir, bpos - apos) < degree / 2)
        {
            return true;
        }
        return false;
    }
    public static bool Sector(Transform point, Transform target, Vector2 dir, float radius, float degree)
    {
        return Sector(point.position, target.position, dir, radius, degree);
    }
}
