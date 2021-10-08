using UnityEngine;

public class FPS : MonoSingleton<FPS>
{
    private float time;
    private int frameCount;
    private string showText;

    private Vector2 normSize = new Vector2(60, 40);
    private Rect rect = new Rect(15, 5, 60, 40);

    protected override void Awake()
    {
        if (CheckInstanceAndDestroy())
        {
            return;
        }
        base.Awake();
        showText = "fps: 0";
    }

    private bool isLeftTop = true;
    public void LeftTop()
    {
        isLeftTop = true;
        rect = new Rect(new Vector2(15, 5), normSize);
    }
    public void RightTop()
    {
        isLeftTop = false;
        rect = new Rect(new Vector2(Screen.width - 60, 5), normSize);
    }

    private void OnGUI()
    {
        if (!isLeftTop)
        {
            rect = new Rect(new Vector2(Screen.width - 60, 5), normSize);
        }
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