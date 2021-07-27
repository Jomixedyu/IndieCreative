using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Texture2DUtility
{
    /// <summary>
    /// 将Texture2D写入磁盘
    /// </summary>
    /// <param name="path"></param>
    /// <param name="tex"></param>
    public static void WriteTexture(string path, Texture2D tex)
    {
        File.WriteAllBytes(path, tex.EncodeToPNG());
    }
    /// <summary>
    /// 从磁盘加载Texture2D
    /// </summary>
    /// <param name="path"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Texture2D LoadTexture(string path, int width, int height)
    {
        Texture2D tex2d = new Texture2D(width, height);
        tex2d.LoadImage(File.ReadAllBytes(path));
        return tex2d;
    }
    /// <summary>
    /// 从磁盘加载Sprite
    /// </summary>
    /// <param name="path"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Sprite LoadSprite(string path, int width, int height)
    {
        Texture2D tex2d = LoadTexture(path, width, height);
        Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, width, height), Vector2.zero);
        return sprite;
    }
    /// <summary>
    /// 从Texture2D获取一个Sprite
    /// </summary>
    /// <param name="tex"></param>
    /// <returns></returns>
    public static Sprite Texture2dToSprite(this Texture2D tex)
    {
        Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        return s;
    }
    /// <summary>
    /// 缩放Texture
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetWidth"></param>
    /// <param name="targetHeight"></param>
    /// <returns></returns>
    public static Texture2D ScaleTexture(this Texture2D source, int targetWidth, int targetHeight)
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
