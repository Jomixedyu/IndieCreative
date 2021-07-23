using System.Collections.Generic;
using UnityEngine;


public class AudioPool : MonoBehaviour
{
    protected class AudioState
    {
        public AudioSource audioSource;
        public AudioClip clip;
        public bool isUsed;
        /// <summary>
        /// 如果name为null则为匿名音频
        /// </summary>
        public string name;
        public float createTime;
        public float endTime;
        public bool isLoop;
    }

    [SerializeField]
    private int maxCount = 5;
    public int MaxCount { get => maxCount; }
    protected List<AudioState> pool;

    private float volume = 1f;
    public float Volume
    {
        get => volume;
        set
        {
            if (value > 1) value = 1;
            if (value < 0) value = 0;
            volume = value;
            foreach (var item in pool)
            {
                item.audioSource.volume = value;
            }
        }
    }

    private void Awake()
    {
        pool = new List<AudioState>(maxCount + 1);
        //创建对象
        for (int i = 0; i < maxCount; i++)
        {
            AudioSource ap = gameObject.AddComponent<AudioSource>();
            ap.volume = volume;
            AudioState aus = new AudioState() { audioSource = ap };
            pool.Add(aus);
        }
    }

    private void Update()
    {
        if (Time.frameCount % 15 == 0)
        {
            foreach (var item in pool)
            {
                if (!item.audioSource.isPlaying)
                {
                    ReleaseAudio(item);
                }
            }
        }

    }
    /// <summary>
    /// 播放一个音频
    /// </summary>
    /// <param name="name"></param>
    /// <param name="ac"></param>
    /// <param name="isLoop"></param>
    public void Play(string name, AudioClip ac, bool isLoop)
    {
        if (ac == null)
        {
            Debug.LogError("audioclip is null");
            return;
        }

        AudioState aus = GetUsableAudio();
        aus.name = name;
        aus.isLoop = isLoop;
        aus.clip = ac;
        aus.createTime = Time.time;
        aus.endTime = aus.createTime + ac.length;
        aus.isUsed = true;

        aus.audioSource.clip = ac;
        aus.audioSource.loop = isLoop;
        aus.audioSource.Play();
    }
    /// <summary>
    /// 播放一次匿名音频
    /// </summary>
    /// <param name="ac"></param>
    public void Play(AudioClip ac)
    {
        Play(null, ac, false);
    }

    /// <summary>
    /// 停止某个音频
    /// </summary>
    /// <param name="name"></param>
    public void Stop(string name)
    {
        foreach (var item in pool)
        {
            if (item.name == name)
            {
                ReleaseAudio(item);
            }
        }
    }
    /// <summary>
    /// 停止并释放所有音频
    /// </summary>
    public void StopAll()
    {
        foreach (var item in pool)
        {
            ReleaseAudio(item);
        }
    }

    protected AudioState GetUsableAudio()
    {
        //先找空闲的，没有空闲的按时间排，查找非循环和最早创建的
        foreach (var item in pool)
        {
            if (!item.isUsed) return item;
        }

        pool.Sort((AudioState x, AudioState y) =>
        {
            var r = x.isLoop.CompareTo(y.isLoop);
            if (r != 0)
            {
                return r;
            }
            return x.createTime.CompareTo(y.createTime);
        });
        return pool[0];
    }
    protected void ReleaseAudio(AudioState aus)
    {
        aus.audioSource.Stop();
        aus.audioSource.clip = null;
        aus.clip = null;
        aus.isUsed = false;
        aus.name = null;
    }
}



