using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Texture2DUtility
{
    public static void WriteTexture(string path, Texture2D tex)
    {
        File.WriteAllBytes(path, tex.EncodeToPNG());
    }
    public static Texture2D LoadTexture(string path, int width, int height)
    {
        Texture2D tex2d = new Texture2D(width, height);
        tex2d.LoadImage(File.ReadAllBytes(path));
        return tex2d;
    }
    public static Sprite LoadSprite(string path, int width, int height)
    {
        Texture2D tex2d = LoadTexture(path, width, height);
        Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, width, height), Vector2.zero);
        return sprite;
    }

    public static Sprite Texture2dToSprite(Texture2D tex)
    {
        Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        return s;
    }
    public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }

        result.Apply();
        return result;
    }
}
