using System;
using System.Collections.Generic;
using UnityEngine;

namespace JxUnity.Timers
{
    /// <summary>
    /// 循环计时器
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// 执行间隔，单位：秒
        /// </summary>
        public float Interval { get; set; } = 0;

        /// <summary>
        /// 是否使用不受缩放影响的时间
        /// </summary>
        public bool IsUnscale { get; set; } = false;

        private float delta = 0;
        private List<Action> action;
        private int waitExecuteIndex = 0;

        private bool isRunning;

        /// <summary>
        /// 计时器
        /// </summary>
        /// <param name="interval">时间间隔，单位：秒</param>
        /// <param name="isUnscale">是否受Unity时间缩放所影响</param>
        public Timer(float interval, Action callback)
        {
            this.Interval = interval;
            this.action = new List<Action>(1);
            this.action.Add(callback);
            this.isRunning = false;
        }

        /// <summary>
        /// 顺序计时器
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="queue"></param>
        public Timer(float interval, params Action[] queue)
        {
            this.Interval = interval;
            this.action = new List<Action>(queue);
            this.isRunning = false;
        }

        /// <summary>
        /// 添加至顺序计时器队列
        /// </summary>
        /// <param name="callback"></param>
        public void AddToQueue(Action callback)
        {
            this.action.Add(callback);
        }

        /// <summary>
        /// 创建计时器
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Timer Create(float interval, Action callback)
        {
            return new Timer(interval, callback);
        }

        /// <summary>
        /// 创建顺序队列计时器，循环依次执行队列中的方法。
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static Timer CreateQueue(float interval, params Action[] queue)
        {
            return new Timer(interval, queue);
        }

        public void Start()
        {
            if (this.isRunning)
            {
                throw new Exception("timer is running");
            }
            this.delta = 0f;
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
            this.delta += this.IsUnscale ? Time.unscaledDeltaTime : Time.deltaTime;

            while (this.delta >= this.Interval)
            {
                this.delta -= this.Interval;

                this.action[this.waitExecuteIndex].Invoke();
                this.waitExecuteIndex = (this.waitExecuteIndex + 1) % this.action.Count;
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