using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace JxUnity.Tasks
{
    public class AsyncTaskAwaiter : INotifyCompletion
    {
        public bool IsCompleted { get; private set; }
        public void OnCompleted(Action continuation)
        {
            continuation?.Invoke();
        }
        public void GetResult()
        {

        }
    }

    public class AsyncTask
    {
        Action act;
        public static AsyncTask Run(Action action)
        {
            var asyncTask = new AsyncTask();
            asyncTask.act = action;
            return asyncTask;
        }

        public AsyncTaskAwaiter GetAwaiter()
        {
            return new AsyncTaskAwaiter();
        }


        /// <summary>
        /// 异步执行方法
        /// </summary>
        /// <param name="syncMethod">非unity线程</param>
        /// <param name="unityThreadCallBack">unity线程回调</param>
        public static void ExecuteAysncMethod(Action syncMethod, Action unityThreadCallBack)
        {
            Task.Run(() =>
            {
                syncMethod?.Invoke();
                AsyncTaskQueueMono.Instance.Add(unityThreadCallBack);
            });
        }

        
    }

    internal class AsyncTaskQueueMono : MonoBehaviour
    {
        private static AsyncTaskQueueMono _instance = null;
        public static AsyncTaskQueueMono Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("__m_" + nameof(AsyncTaskQueueMono));
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<AsyncTaskQueueMono>();
                }
                return _instance;
            }
        }

        private List<Action> actions;

        private void Awake()
        {
            this.actions = new List<Action>();
        }

        public void Add(Action action)
        {
            this.actions.Add(action);
        }

        private void LateUpdate()
        {
            if (this.actions.Count !=0)
            {
                foreach (var item in this.actions)
                {
                    item.Invoke();
                }
                this.actions.Clear();
            }
        }

    }
}
