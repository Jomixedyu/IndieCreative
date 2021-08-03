using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JxUnity.Jxugui
{
    /// <summary>
    /// 顶级UI管理器
    /// </summary>
    public sealed class JuiManager : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private bool IsDontDestroyOnInit = true;
        private static JuiManager mInstance = null;

        public static JuiManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = GameObject.FindObjectOfType(typeof(JuiManager)) as JuiManager;
                    if (mInstance == null)
                    {
                        Debug.LogError("juimanager instance not found.");
                        return null;
                    }
                    if (mInstance.IsDontDestroyOnInit)
                    {
                        DontDestroyOnLoad(mInstance.gameObject);
                    }
                }
                return mInstance;
            }
        }
        public static bool HasInstance
        {
            get => mInstance != null;
        }

        private event Action UpdateHandler;
        private struct UpdateQueueData
        {
            public Action Func;
            public bool IsAdd;

            public UpdateQueueData(Action func, bool isAdd)
            {
                this.Func = func;
                this.IsAdd = isAdd;
            }
        }
        private List<UpdateQueueData> updateOperateQueue;

        private List<JuiBaseAbstract> uiShowStack;
        private Dictionary<string, JuiBaseAbstract> ui;

        private static Dictionary<string, UIInfo> uiTypes;
        private class UIInfo
        {
            public string TypeName;
            public string UIName;
            public Type UIType;
            public JuiPanelAttribute Attr;
            public override string ToString()
            {
                return TypeName;
            }
        }


        public void AddUpdateHandler(Action action)
        {
            this.updateOperateQueue.Add(new UpdateQueueData(action, true));
        }
        public void RemoveUpdateHandler(Action action)
        {
            this.updateOperateQueue.Add(new UpdateQueueData(action, false));
        }


        private void AutoBind(UIInfo uiInfo)
        {
            bool hasRealUI = Instance.transform.Find(uiInfo.TypeName) != null;
            if (hasRealUI)
            {
                this.RegisterUI(uiInfo.TypeName);
                if (uiInfo.Attr.IsPreBind)
                {
                    Type genericType = typeof(JuiBase<>).MakeGenericType(new Type[] { uiInfo.UIType });
                    var method = genericType.GetMethod("GetInstance", BindingFlags.Public | BindingFlags.Static);
                    //auto SetUI
                    method.Invoke(null, null);
                }
            }
        }

        private void Awake()
        {
            if (HasInstance)
            {
                //move
                if (transform.childCount > 0)
                {
                    //create temp
                    foreach (var item in uiTypes)
                    {
                        string uiName = item.Key;
                        UIInfo uiInfo = item.Value;
                        if (Instance.Exist(uiName))
                        {
                            continue;
                        }
                        //move
                        Transform t = transform.Find(uiInfo.TypeName);

                        if (t != null)
                        {
                            t.SetParent(Instance.transform);
                            this.AutoBind(uiInfo);
                        }
                    }
                }
                Destroy(this.gameObject);
                return;
            }

            this.uiShowStack = new List<JuiBaseAbstract>();
            this.ui = new Dictionary<string, JuiBaseAbstract>();
            this.updateOperateQueue = new List<UpdateQueueData>();

            if (uiTypes == null)
            {
                uiTypes = new Dictionary<string, UIInfo>();
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (type.IsDefined(typeof(JuiPanelAttribute)))
                    {
                        var attr = type.GetCustomAttribute<JuiPanelAttribute>();
                        if (attr.Name == null)
                        {
                            attr.Name = type.Name;
                        }
                        uiTypes.Add(type.Name, new UIInfo()
                        {
                            TypeName = type.Name,
                            UIName = attr.Name,
                            UIType = type,
                            Attr = attr
                        });
                    }
                }
            }

            //auto bind
            foreach (var item in uiTypes)
            {
                this.AutoBind(item.Value);
            }
        }
        public static JuiPanelAttribute GetUIAttribute(Type type)
        {
            var name = type.Name;
            if (!uiTypes.ContainsKey(name))
            {
                return null;
            }
            return uiTypes[name].Attr;
        }

        [SerializeField]
        [Tooltip("是否启用导航重新选择，启用后鼠标点击和隐藏显示将不会导致丢失导航")]
        private bool enableReselect;
        /// <summary>
        /// 是否启用导航重新选择，启用后鼠标点击和隐藏显示将不会导致丢失导航
        /// </summary>
        public bool EnableReselect { get => enableReselect; set => enableReselect = value; }
        private GameObject lastSelected;
        private bool lastSelectedlastActive = false;
        private void Reselect()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }
            else
            {
                lastSelected = EventSystem.current.currentSelectedGameObject;
            }
            if (lastSelected != null)
            {
                if (lastSelected.activeInHierarchy && !lastSelectedlastActive)
                {
                    lastSelected.GetComponent<Selectable>().OnSelect(null);
                    Debug.Log("Bababa");
                }
                lastSelectedlastActive = lastSelected.activeInHierarchy;
            }

        }

        private void Update()
        {
            if (EnableReselect)
            {
                this.Reselect();
            }
            this.UpdateHandler?.Invoke();
            if (this.updateOperateQueue.Count > 0)
            {
                foreach (var item in this.updateOperateQueue)
                {
                    if (item.IsAdd)
                    {
                        this.UpdateHandler += item.Func;
                    }
                    else
                    {
                        this.UpdateHandler -= item.Func;
                    }
                }
                this.updateOperateQueue.Clear();
            }
        }

        public void Push(JuiBaseAbstract obj)
        {
            this.GetFocus()?.SendMessage(JuiAbstract.MessageType.LostFocus);
            this.uiShowStack.Add(obj);
            obj.SendMessage(JuiAbstract.MessageType.Focus);
        }
        public void Pop(JuiBaseAbstract obj)
        {
            if (obj != this.GetFocus())
            {
                this.uiShowStack.Remove(obj);
                return;
            }
            obj.SendMessage(JuiAbstract.MessageType.LostFocus);
            this.uiShowStack.Remove(obj);
            this.GetFocus()?.SendMessage(JuiAbstract.MessageType.Focus);
        }

        /// <summary>
        /// 获取Focus
        /// </summary>
        /// <returns></returns>
        public JuiBaseAbstract GetFocus()
        {
            if (this.uiShowStack.Count == 0)
            {
                return null;
            }
            return this.uiShowStack[this.uiShowStack.Count - 1];
        }
        /// <summary>
        /// 设置为Focus
        /// </summary>
        /// <param name="type"></param>
        public void SetFocus(JuiBaseAbstract obj)
        {
            if (obj == this.GetFocus())
            {
                return;
            }
            this.uiShowStack.Remove(obj);
            this.GetFocus()?.SendMessage(JuiAbstract.MessageType.LostFocus);
            this.uiShowStack.Add(obj);
            obj.SendMessage(JuiAbstract.MessageType.Focus);
        }

        public void RegisterUI(string name)
        {
            this.ui.Add(name, null);
        }
        public void SetUIInstance(string name, JuiBaseAbstract ui)
        {
            this.ui[name] = ui;
        }
        public JuiBaseAbstract GetUIInstance(string name)
        {
            return this.ui[name];
        }
        public Dictionary<string, JuiBaseAbstract> GetAllUI()
        {
            return this.ui;
        }
        public bool Exist(string ui)
        {
            return this.ui.ContainsKey(ui);
        }
        public bool HasUIInstance(string ui)
        {
            JuiBaseAbstract uiinst = null;
            this.ui.TryGetValue(ui, out uiinst);
            return uiinst != null;
        }

        public static Func<string, GameObject> LoadResourceHandler { get; set; }

        public GameObject LoadResource(string path)
        {
            if (LoadResourceHandler == null)
            {
                throw new System.NotImplementedException("LoadResourceHandler is null: " + path);
            }
            return LoadResourceHandler.Invoke(path);
        }

        private bool isDisposed = false;
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }
            //unload all ui
            this.uiShowStack?.Clear();

            if (this.ui != null)
            {
                foreach (var item in this.ui)
                {
                    item.Value.Dispose();
                }
                this.ui?.Clear();
            }
        }
    }
}