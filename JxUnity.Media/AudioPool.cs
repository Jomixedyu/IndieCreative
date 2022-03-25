using System;
using System.Collections.Generic;
using UnityEngine;

namespace JxUnity.Media
{
    public class AudioPool : MonoBehaviour
    {
        protected class AudioState : IComparable<AudioState>
        {
            //max volume
            private float volume;
            public float Volume
            {
                get => volume;
                set
                {
                    if (this.audioSource.volume > value)
                    {
                        this.audioSource.volume = value;
                    }
                    this.volume = value;
                }
            }

            public bool IsPlaying { get => this.audioSource.isPlaying; }
            public bool IsUsed { get; private set; }

            private AudioSource audioSource;

            /// <summary>
            /// 如果name为null则为匿名音频
            /// </summary>
            public string Name { get; private set; }
            private float createTime;
            private float endTime;
            public bool IsLoop { get; private set; }
            private bool isFade;

            private bool isToFade;

            public AudioState(AudioSource audioSource)
            {
                this.audioSource = audioSource;
            }

            public void Play(AudioClip ac, string name = null, bool isLoop = false, bool isFade = false)
            {
                if (ac == null)
                {
                    Debug.LogError("audioclip is null");
                    return;
                }
                this.isToFade = false;
                this.Name = name;
                this.IsLoop = isLoop;
                this.isFade = isFade;
                this.createTime = Time.time;

                this.endTime = this.createTime + ac.length;

                this.audioSource.clip = ac;
                this.audioSource.loop = isLoop;
                this.audioSource.volume = this.volume;
                this.audioSource.Play();

                this.IsUsed = true;
            }
            public void Release()
            {
                this.Name = null;
                this.IsLoop = false;
                this.audioSource.Stop();
                this.audioSource.clip = null;
                this.IsUsed = false;
            }
            public void Stop()
            {
                if (!this.isFade)
                {
                    this.Release();
                }
                else
                {
                    this.isToFade = true;
                }
            }

            public void Update()
            {
                if (this.isFade && this.isToFade)
                {
                    var v = this.audioSource.volume;
                    v = Math.Max(0, v - Time.unscaledDeltaTime);
                    this.audioSource.volume = v;

                    if (v <= 0)
                    {
                        this.Release();
                    }
                }

                if (!this.IsPlaying)
                {
                    this.Release();
                }
            }

            public int CompareTo(AudioState other)
            {
                var r = this.IsLoop.CompareTo(other.IsLoop);
                if (r != 0)
                {
                    return r;
                }
                return this.endTime.CompareTo(other.endTime);
            }
        }

        [SerializeField]
        protected int maxCount = 5;
        public int MaxCount { get => maxCount; }
        protected List<AudioState> pool;
        [SerializeField]
        private bool[] poolState;
        //[SerializeField][Range(0f, 1f)]
        //private float m_volume = 1f;
        [SerializeField]
        private float volume = 1f;
        public float Volume
        {
            get => volume;
            set
            {
                volume = Mathf.Clamp(value, 0, 1);
                foreach (var item in pool)
                {
                    item.Volume = volume;
                }
            }
        }

        protected void Awake()
        {
            pool = new List<AudioState>(maxCount);
            poolState = new bool[MaxCount];
            //创建对象
            for (int i = 0; i < maxCount; i++)
            {
                AudioSource ap = gameObject.AddComponent<AudioSource>();
                ap.volume = volume;
                AudioState aus = new AudioState(ap);
                pool.Add(aus);
            }
        }

        protected void Update()
        {
            for (int i = 0; i < this.pool.Count; i++)
            {
                var item = this.pool[i];
                if (item.IsUsed)
                {
                    item.Update();
                }
            }
            for (int i = 0; i < MaxCount; i++)
            {
                poolState[i] = this.pool[i].IsUsed;
            }
        }
        /// <summary>
        /// 播放一个音频
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ac"></param>
        /// <param name="isLoop"></param>
        public void Play(AudioClip ac, string name = null, bool isLoop = false, bool isFade = false)
        {
            if (ac == null)
            {
                Debug.LogError("audioclip is null");
                return;
            }

            AudioState aus = GetUsableAudio();
            aus.Volume = volume;
            aus.Play(ac, name, isLoop, isFade);
        }

        /// <summary>
        /// 停止某个音频
        /// </summary>
        /// <param name="name"></param>
        public void Stop(string name)
        {
            foreach (var item in pool)
            {
                if (item.Name == name)
                {
                    item.Stop();
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
                item.Stop();
            }
        }

        public void Clear()
        {
            foreach (var item in this.pool)
            {
                item.Release();
            }
        }

        protected AudioState GetUsableAudio()
        {
            //先找空闲的，没有空闲的按时间排，查找非循环和最早创建的
            foreach (var item in pool)
            {
                if (!item.IsUsed) return item;
            }

            pool.Sort((x, y) => x.CompareTo(y));
            return pool[0];
        }
    }
}