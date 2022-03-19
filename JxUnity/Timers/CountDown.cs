using System;
using System.Collections.Generic;
using UnityEngine;

namespace JxUnity.Timers
{
    public class CountDown
    {
        private bool isUsed;
        public bool IsUsed => isUsed;

        public event Action OnCountDownEnd;

        public event Action<float> NotifyCallback;
        public float NotifyInterval { get; set; } = 0;

        private float timeLength;

        public float Time => timeLength;
        public int TimeInt => (int)Time;

        private float deltaCount;

        public CountDown(float time)
        {
            this.timeLength = time;
        }
        public void Reset(float time)
        {
            Pause();
            deltaCount = 0;
            timeLength = time;
        }
        public void Start()
        {
            if (!isUsed)
            {
                CountDownMono.Instance.OnUpdate += OnUpdate;
            }
            isUsed = true;
        }
        public void Pause()
        {
            isUsed = false;
            CountDownMono.Instance.OnUpdate -= OnUpdate;
        }
        public void End()
        {
            timeLength = 0;
            if (isUsed)
            {
                CountDownMono.Instance.OnUpdate -= OnUpdate;
                if (this.NotifyInterval > 0)
                {
                    this.NotifyCallback?.Invoke(0);
                }
                OnCountDownEnd?.Invoke();
            }
        }

        public void AddTime(float time)
        {
            this.timeLength += time;
        }

        
        protected void OnUpdate()
        {
            float delta = UnityEngine.Time.deltaTime;

            if (NotifyInterval > 0)
            {
                deltaCount += delta;
                while (deltaCount >= NotifyInterval)
                {
                    deltaCount -= NotifyInterval;
                    NotifyCallback?.Invoke(timeLength + deltaCount);
                }
            }

            timeLength -= delta;
            if (timeLength <= 0)
            {
                End();
            }
        }

    }

    internal class CountDownMono : MonoBehaviour
    {
        private static CountDownMono instance;
        public static CountDownMono Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject($"__m_${nameof(CountDownMono)}");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<CountDownMono>();
                }
                return instance;
            }
        }

        public event Action OnUpdate;

        private void Update()
        {
            this.OnUpdate?.Invoke();
        }

    }
}
