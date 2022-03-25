using System;
using UnityEngine;


public static class MathExtension
{
    public static Color Saturate(this Color self)
    {
        return new Color(Mathf.Clamp01(self.r), Mathf.Clamp01(self.g), Mathf.Clamp01(self.b), Mathf.Clamp01(self.a));
    }
    public static Color Add(this Color self, float scalar)
    {
        return new Color(self.r + scalar, self.g + scalar, self.b + scalar, self.a + scalar);
    }
}

