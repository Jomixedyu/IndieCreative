using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIEff_SeqImage : MonoBehaviour
{
    private Image image = null;

    [SerializeField]
    private float frameSpeed = 24f;
    public float FrameSpeed { get => frameSpeed; }

    [SerializeField]
    private bool isPlaying = false;
    public bool IsPlaying { get => isPlaying; }

    [SerializeField]
    private int index = 0;
    public int Index { get => index; }

    [SerializeField]
    private bool isAutoPlay = true;
    public bool IsAutoPlay { get => IsAutoPlay; }

    [SerializeField]
    private Sprite[] seqSprites = null;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    private void Start()
    {
        if (isAutoPlay) Play();
    }
    public void Play()
    {
        isPlaying = true;
    }
    public void Stop()
    {
        isPlaying = false;
    }

    private float count = 0;
    private void Update()
    {
        if (isPlaying)
        {
            if(seqSprites == null || seqSprites.Length == 0)
            {
                return;
            }
            count += Time.deltaTime;
            float frameTime = 1f / this.frameSpeed;
            if(count >= frameTime)
            {
                index++;
                if (seqSprites.Length == index) index = 0;
                image.sprite = seqSprites[index];
                count = 0;
            }
        }
    }

}
