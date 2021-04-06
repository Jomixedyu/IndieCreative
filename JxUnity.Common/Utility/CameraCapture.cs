using System;
using UnityEngine;

public class CameraCapture
{
    public static Texture2D Capture(Camera camera)
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
