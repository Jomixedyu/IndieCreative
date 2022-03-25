using System;
using UnityEngine;

public class UIEff_Twinkle : UIEff_AlphaTransition
{
    [SerializeField]
    private bool isPlaying = false;
    public bool IsPlaying { get => isPlaying; set => isPlaying = value; }
    protected override void Awake() 
    {
        if(base.playOnAwake)
        {
            isPlaying = true;
        }
        base.Awake();
    }
    protected override void OnHide()
    {
        if (isPlaying) Show();
    }
    protected override void OnShow()
    {
        Hide();
    }
    public void Play()
    {
        isPlaying = true;
        Show();
    }
    public void Stop()
    {
        isPlaying = false;
    }
}
