using System;
using UnityEngine;

/// <summary>
/// 摄像头捕捉器
/// </summary>
public static class CameraCapture
{
    /// <summary>
    /// 使用主相机捕捉图像
    /// </summary>
    /// <returns></returns>
    public static Texture2D Capture()
    {
        return Camera.main.Capture();
    }
    /// <summary>
    /// 捕捉图像
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static Texture2D Capture(this Camera camera)
    {
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        RenderTexture rtex = new RenderTexture((int)rect.width, (int)rect.height, 0);

        camera.targetTexture = rtex;
        camera.Render();

        RenderTexture.active = rtex;
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);

        tex.ReadPixels(rect, 0, 0);
        tex.Apply();

        camera.targetTexture = null;
        RenderTexture.active = null;

        UnityEngine.Object.Destroy(rtex);

        return tex;
    }
    /// <summary>
    /// 按摄像机层叠捕捉图像
    /// </summary>
    /// <param name="cameras"></param>
    /// <returns></returns>
    public static Texture2D Captures(Camera[] cameras)
    {
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        RenderTexture rtex = new RenderTexture((int)rect.width, (int)rect.height, 0);

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].targetTexture = rtex;
            cameras[i].Render();
            cameras[i].targetTexture = null;
        }

        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);

        RenderTexture.active = rtex;
        tex.ReadPixels(rect, 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        UnityEngine.Object.Destroy(rtex);

        return tex;
    }
}
