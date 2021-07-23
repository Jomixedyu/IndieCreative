using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoSingleton<FPS>
{
    private float time;
    private int frameCount;
    private string showText;

    private Rect rect;
    private Vector2 normSize;

    private void Awake()
    {
        if (CheckInstanceAndDestroy())
        {
            return;
        }
        showText = "fps: 0";
        normSize = new Vector2(60, 40);
    }
    public void LeftTop()
    {
        rect = new Rect(new Vector2(15, 5), normSize);
    }
    public void RightTop()
    {
        rect = new Rect(new Vector2(Screen.width - 60, 5), normSize);
    }
    public void SetPosition(Vector2 vector2)
    {
        rect = new Rect(vector2, normSize);
    }
    private void OnGUI()
    {
        GUI.Label(rect, this.showText);
    }

    private void Update()
    {
        time += Time.unscaledDeltaTime;
        frameCount++;
        if (time >= 1 && frameCount >= 1)
        {
            int fps = frameCount / (int)time;
            time = 0;
            frameCount = 0;
            SetText(fps.ToString());
        }
    }

    private void SetText(string text)
    {
        this.showText = "fps: " + text;
    }
}
