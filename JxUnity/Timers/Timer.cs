using System;
using UnityEngine;

namespace JxUnity.Timers
{
    public class Timer
    {
        public float Interval { get; set; }
        public bool IsUnscale { get; set; }

        private float delta = 0;
        private Action action;

        private bool isRunning;

        /// <summary>
        /// 计时器
        /// </summary>
        /// <param name="interval">时间间隔</param>
        /// <param name="isUnscale">是否受Unity时间缩放所影响</param>
        public Timer(float interval = 0, bool isUnscale = false)
        {
            this.Interval = interval;
            this.IsUnscale = isUnscale;
            this.isRunning = false;
        }

        public static Timer Create(float interval = 0, bool isUnscale = false)
        {
            return new Timer(interval, isUnscale);
        }

        public void Start(Action cb)
        {
            if (this.isRunning)
            {
                throw new Exception("timer is running");
            }
            this.delta = 0f;
            this.action = cb;
            this.isRunning = true;
            TimerMono.Instance.OnUpdate += this.Instance_OnUpdate;
        }

        public void Stop()
        {
            if (!this.isRunning)
            {
                return;
            }
            this.isRunning = false;
            TimerMono.Instance.OnUpdate -= this.Instance_OnUpdate;
        }

        private void Instance_OnUpdate()
        {
            if (this.IsUnscale)
            {
                this.delta += Time.unscaledDeltaTime;
            }
            else
            {
                this.delta += Time.deltaTime;
            }

            while (this.delta >= this.Interval)
            {
                this.delta -= this.Interval;
                this.action?.Invoke();
            }

        }

    }

    internal class TimerMono : MonoBehaviour
    {
        private static TimerMono instance;
        public static TimerMono Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject($"__m_{nameof(TimerMono)}").AddComponent<TimerMono>();
                }
                return instance;
            }
        }

        internal event Action OnUpdate;
        private void Update()
        {
            this.OnUpdate();
        }
    }
}