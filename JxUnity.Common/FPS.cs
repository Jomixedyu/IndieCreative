using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    private float time;
    private int frameCount;
    private string showText = "fps: 0";

    private void OnGUI()
    {
        GUI.Label(new Rect(15, 5, 60, 40), this.showText);
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
