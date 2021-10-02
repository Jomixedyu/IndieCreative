using System.Collections;
using UnityEngine;

namespace JxUnity.Utility
{
    public class CoroutineTask
    {
        public bool Running { get; private set; }
        public bool Paused { get; private set; }
        private IEnumerator coroutine;

        public CoroutineTask(IEnumerator coroutine)
        {
            this.coroutine = coroutine;
        }

        public void Start()
        {
            Running = true;
            Paused = false;
            CoroutineTaskMono.Instance.StartCoroutine(this.CallWrapper());
        }
        public void Stop()
        {
            Running = false;
        }
        public void Pause()
        {
            Paused = true;
        }
        public void Resume()
        {
            if (!Running)
            {
                Start();
            }
            Paused = false;
        }

        IEnumerator CallWrapper()
        {
            yield return null;
            IEnumerator e = this.coroutine;
            while (Running)
            {
                if (Paused)
                {
                    yield return null;
                }
                else
                {
                    if (e != null && e.MoveNext())
                    {
                        yield return e.Current;
                    }
                    else
                    {
                        Running = false;
                    }
                }
            }
        }

        public static CoroutineTask Create(IEnumerator c)
        {
            return new CoroutineTask(c);
        }

        public static void StartCoroutine(IEnumerator routine)
        {
            CoroutineTaskMono.Instance.StartCoroutine(routine);
        }
    }

    internal class CoroutineTaskMono : MonoBehaviour
    {
        private static CoroutineTaskMono mono;
        public static CoroutineTaskMono Instance
        {
            get
            {
                if (mono == null)
                {
                    var go = new GameObject($"__m_{nameof(CoroutineTaskMono)}");
                    DontDestroyOnLoad(go);
                    mono = go.AddComponent<CoroutineTaskMono>();
                }
                return mono;
            }
        }
    }
}
