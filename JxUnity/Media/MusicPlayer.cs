using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip clip;

    public AudioClip Clip { get => clip; }

    [SerializeField]
    private bool isLoop = false;
    public bool IsLoop { get => isLoop; set => isLoop = value; }

    [SerializeField]
    private bool isPlaying = false;
    public bool IsPlaying { get => isPlaying; }

    public event Action OnEndHandler;

    [SerializeField]
    private float transision = 1f;
    public float Transision { get => transision; }

    //最大音量 非实际音量
    private float volume = 1f;
    public float Volume
    {
        get => volume;
        set
        {
            value = Math.Max(Math.Min(value, 1), 0);
            if (!isToMax && !isToMin)
            {
                audioSource.volume = value;
            }
            volume = value;
        }
    }

    private Action transAct;
    private bool isToMax = false;
    private bool isToMin = false;

    [SerializeField]
    private float playedTime = 0;
    [SerializeField]
    private float clipTime = 0;

    private static MusicPlayer instance;
    public static MusicPlayer GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<MusicPlayer>();
            if (instance == null)
            {
                instance = new GameObject(nameof(MusicPlayer)).AddComponent<MusicPlayer>();
            }
        }
        return instance;
    }
    
    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;

        if (audioSource.playOnAwake)
        {
            this.Play();
        }
    }

    private void Update()
    {
        if (isToMax)
        {
            if (audioSource.volume < volume)
            {
                audioSource.volume += Time.deltaTime * 1 / transision;
            }
            else
            {
                audioSource.volume = volume;
                isToMax = false;
                OnToMax();
            }
        }
        else if (isToMin)
        {
            if (audioSource.volume > 0)
            {
                audioSource.volume -= Time.deltaTime * 1 / transision;
            }
            else
            {
                audioSource.volume = 0;
                isToMin = false;
                OnToMin();
            }
        }

        if (this.isPlaying && Time.frameCount % 60 == 0)
        {
            playedTime = audioSource.time;
            if (audioSource.time == 0)
                OnEnd();
        }
    }

    private void ToMin()
    {
        isToMin = true;
        isToMax = false;
    }
    private void ToMax()
    {
        isToMin = false;
        isToMax = true;
    }
    private void OnToMin()
    {
        if (transAct != null)
        {
            transAct.Invoke();
            transAct = null;
        }
    }
    private void OnToMax()
    {

    }


    private void OnEnd()
    {
        OnEndHandler?.Invoke();
        if (isLoop)
        {
            audioSource.time = 0;
            this.Play();
        }
        else
        {
            isPlaying = false;
        }

    }

    public void Play()
    {
        isPlaying = true;
        audioSource.Play();
        ToMax();
    }
    public void Pause()
    {
        isPlaying = false;

        transAct = audioSource.Pause;
        ToMin();
    }

    public void Play(AudioClip audioClip)
    {
        clip = audioClip;
        if (isPlaying)
        {
            transAct = () =>
            {
                clipTime = clip.length;
                audioSource.clip = clip;
                audioSource.Play();
                ToMax();
            };
            ToMin();
        }
        else
        {

            isPlaying = true;
            clipTime = clip.length;
            audioSource.clip = clip;
            audioSource.Play();
            ToMax();
        }
    }

    public void Stop()
    {
        isPlaying = false;

        transAct = audioSource.Stop;
        ToMin();
    }
}
