using System;
using UnityEngine;

public static class Measure3D
{
    public static Vector3 RemoveYAxis(this Vector3 v3) => new Vector3(v3.x, 0, v3.z);

    /// <summary>
    /// 扇形检测
    /// </summary>
    /// <param name="from">源</param>
    /// <param name="target">目标</param>
    /// <param name="radius">半径</param>
    /// <param name="degree">角度</param>
    /// <param name="height">高度</param>
    /// <returns></returns>
    public static bool Sector(Transform from, Transform target,
        float radius = 2.5f, float degree = 120, float height = 2)
    {
        Vector3 aPos = from.position;
        Vector3 bPos = target.position;
        if (Math.Abs(aPos.y - bPos.y) > height) return false;
        aPos.y = 0; bPos.y = 0;
        if (Vector3.Distance(aPos, bPos) > radius) return false;
        if (Vector3.Angle(from.forward, aPos - bPos) < degree / 2) return true;
        return false;
    }
    /// <summary>
    /// 圆柱形范围检测
    /// </summary>
    /// <param name="from">源</param>
    /// <param name="target">目标</param>
    /// <param name="radius">半径</param>
    /// <param name="height">高度</param>
    /// <returns></returns>
    public static bool Cylinder(Transform from, Transform target,
        float radius = 3f, float height = 5)
    {
        Vector3 aPos = from.position;
        Vector3 bPos = target.position;
        if (Vector3.Distance(aPos, bPos) > radius) return false;
        if (aPos.y - bPos.y > height) return false;

        return true;
    }
    /// <summary>
    /// 球形范围检测
    /// </summary>
    /// <param name="from">源</param>
    /// <param name="target">目标</param>
    /// <param name="radius">半径</param>
    /// <returns></returns>
    public static bool Sphere(Transform from, Transform target, float radius)
    {
        return Vector3.Distance(from.position, target.position) < radius;
    }
    /// <summary>
    /// 矩形检测
    /// </summary>
    /// <param name="from">源</param>
    /// <param name="target">目标</param>
    /// <param name="forwardDistance">源前方检测的距离</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <returns></returns>
    public static bool Rect(Transform from, Transform target,
        float forwardDistance = 2.5f, float width = 2, float height = 2)
    {
        if (Math.Abs(from.position.y - target.position.y) > height) return false;

        Vector3 forward = target.forward.RemoveYAxis();
        Vector3 right = target.right.RemoveYAxis();

        Vector3 delta = (from.position - target.position).RemoveYAxis();

        float forwardDotA = Vector3.Dot(forward, delta);
        if (forwardDotA > 0 && forwardDotA <= forwardDistance)
        {
            if (Math.Abs(Vector3.Dot(right, delta)) < width / 2)
                return true;
        }

        return false;
    }
}
