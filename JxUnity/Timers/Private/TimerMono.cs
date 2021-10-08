using System;
using UnityEngine;

namespace JxUnity.Timers
{
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
