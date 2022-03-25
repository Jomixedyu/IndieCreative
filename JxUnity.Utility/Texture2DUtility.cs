using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class Texture2DUtility
{

    private class Texture2DUtilityMono : MonoBehaviour
    {
        private static Texture2DUtilityMono instance;
        public static Texture2DUtilityMono Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject($"__m_{nameof(Texture2DUtilityMono)}").AddComponent<Texture2DUtilityMono>();
                }
                return instance;
            }
        }

        public void Load(string uri, Action<Texture2D> cb)
        {
            StartCoroutine(_Load(uri, cb));
        }

        IEnumerator _Load(string uri, Action<Texture2D> cb)
        {
            UnityWebRequest req = new UnityWebRequest(uri);
            var h = new DownloadHandlerTexture(true);
            req.downloadHandler = h;
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                cb?.Invoke(h.texture);
            }
            else
            {
                cb?.Invoke(null);
            }
        }
    }

    /// <summary>
    /// 将Texture2D写入磁盘
    /// </summary>
    /// <param name="path"></param>
    /// <param name="tex"></param>
    public static void WriteTexture(string path, Texture2D tex)
    {
        File.WriteAllBytes(path, tex.EncodeToPNG());
    }

    public static void WriteTextureAsync(string path, Texture2D tex, Action<Exception> callback)
    {
        var syncCtx = SynchronizationContext.Current;
        Task.Run(() =>
        {
            Exception e = null;
            try
            {
                WriteTexture(path, tex);
            }
            catch (Exception _e)
            {
                e = _e;
            }
            finally
            {
                syncCtx.Send(_e => callback?.Invoke(_e as Exception), e);
            }
        });
    }

    public static Task<Exception> WriteTextureAsync(string path, Texture2D texture)
    {
        return Task<Exception>.Run(() =>
        {
            Exception e = null;
            try
            {
                WriteTexture(path, texture);
            }
            catch (Exception _e)
            {
                e = _e;
            }
            return e;
        });
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

    public static void LoadTextureAsyncFromUri(string uri, Action<Texture2D> cb)
    {
        Texture2DUtilityMono.Instance.Load(uri, cb);
    }

    public static void LoadSpriteAsyncFromUri(string uri, Action<Sprite> cb)
    {
        Texture2DUtilityMono.Instance.Load(uri, t =>
        {
            if (t == null)
            {
                cb?.Invoke(null);
            }
            else
            {
                cb?.Invoke(Texture2dToSprite(t));
            }
        });
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

    public static Color CalcAvgColor(this Texture2D tex, int xSampleStep, int ySampleStep)
    {
        Color rc = Color.black;

        int width = tex.width;
        int height = tex.height;

        int xstep = width / xSampleStep;
        int ystep = height / ySampleStep;

        for (int w = 0; w < width; w += xstep)
        {
            for (int h = 0; h < width; h += ystep)
            {
                Color c = tex.GetPixel(w, h);
                rc += c;
            }
        }
        rc = rc / ((width / xstep) * (height / ystep));
        return rc;
    }
}
