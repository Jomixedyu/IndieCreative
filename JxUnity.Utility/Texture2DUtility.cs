using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    public static void Fill(Texture2D src, Texture2D brush)
    {
        src.SetPixels(brush.GetPixels());
    }
    public static void Draw(Texture2D src, Texture2D brush, Vector2Int pos)
    {
        int width = brush.width;
        int height = brush.height;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var destColor = brush.GetPixel(i, j);
                var srcColor = src.GetPixel(pos.x + i, pos.y + j);
                if (srcColor.a != 0)
                {
                    float alpha = srcColor.a + destColor.a;
                    destColor = Color.Lerp(srcColor, destColor, destColor.a);
                    if (alpha >= 1)
                    {
                        destColor.a = 1;
                    }
                }
                src.SetPixel(pos.x + i, pos.y + j, destColor);
            }
        }
    }
    /// <summary>
    /// 合并Texture2D
    /// </summary>
    /// <param name="objs"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static Texture2D ComposeNew(Texture2D[] objs, Vector2Int[] offset)
    {
        Texture2D t1 = objs[0];
        Texture2D tex = new Texture2D(t1.width, t1.height, t1.format, false);
        Fill(tex, t1);
        for (int i = 1; i < objs.Length; i++)
        {
            Draw(tex, objs[i], offset[i]);
        }
        tex.Apply();
        return tex;
    }

    private static Vector2 TransformToCanvasLocalPosition(Transform current, Canvas canvas)
    {
        var screenPos = canvas.worldCamera.WorldToScreenPoint(current.transform.position);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPos,
            canvas.worldCamera, out localPos);
        return localPos;
    }
    /// <summary>
    /// 以数组顺序从Ugui的Image组件中合并精灵
    /// </summary>
    /// <param name="transforms"></param>
    /// <returns></returns>
    public static Sprite ComposeNewSpriteFromUImage(Transform[] transforms, Canvas canvas)
    {
        Texture2D[] texArr = new Texture2D[transforms.Length];
        Vector2Int[] offsetArr = new Vector2Int[transforms.Length];
        var first = transforms[0];
        for (int i = 0; i < transforms.Length; i++)
        {
            var transform = transforms[i];
            texArr[i] = transform.GetComponent<Image>().mainTexture as Texture2D;
            if (i == 0)
            {
                offsetArr[i] = new Vector2Int(0, 0);
            }
            else
            {
                var fillRect = first.GetComponent<RectTransform>();
                var transRect = transform.GetComponent<RectTransform>();

                var offset = fillRect.sizeDelta * fillRect.pivot - transRect.sizeDelta / 2;
                offset += TransformToCanvasLocalPosition(transform, canvas) - TransformToCanvasLocalPosition(first, canvas);

                offsetArr[i] = new Vector2Int((int)offset.x, (int)offset.y);
            }
        }
        var tex = ComposeNew(texArr, offsetArr);
        return Texture2DUtility.Texture2dToSprite(tex);
    }
}
