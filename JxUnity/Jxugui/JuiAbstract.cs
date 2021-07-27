using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace JxUnity.Jxugui
{
    public abstract class JuiAbstract
    {
        private GameObject _gameObject;
        private Transform _transform;
        public GameObject gameObject { get => _gameObject; }
        public Transform transform
        {
            get => _transform;
            protected set
            {
                _transform = value;
                if (_transform == null)
                    _gameObject = null;
                else
                    _gameObject = _transform.gameObject;
            }
        }

        private JuiPanelBaseAttribute attribute = null;
        protected JuiPanelBaseAttribute attr
        {
            get => attribute;
            set => attribute = value;
        }

        public string Name { get => attr.Name; }

        public virtual bool IsFocusSelf
        {
            get
            {
                if (this.parent != null)
                {
                    return this.parent.GetSubUIFocus() == this;
                }
                return false;
            }
        }

        public virtual bool IsFocus
        {
            get
            {
                if (this.parent == null)
                {
                    return this.IsFocusSelf;
                }
                return this.parent.IsFocus && this.IsFocusSelf;
            }
        }

        public virtual void SetFocus()
        {
            if (this.parent != null)
            {
                this.parent.SetSubUIFocus(this);
            }
        }

        protected JuiAbstract parent;
        public virtual JuiAbstract Parent
        {
            get => parent;
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsShow { get; protected set; }

        private bool isInit = false;

        private List<JuiAbstract> subUis = new List<JuiAbstract>();

        private List<JuiAbstract> subuiShowStack = new List<JuiAbstract>();
        private Action subuiUpdateHandler;
        private struct OperateQueue
        {
            public Action Handler;
            public bool IsAdd;

            public OperateQueue(Action handler, bool isAdd)
            {
                this.Handler = handler;
                this.IsAdd = isAdd;
            }
        }

        private List<OperateQueue> operateQueue = new List<OperateQueue>();

        protected void AddSubUI(JuiAbstract sub)
        {
            this.subUis.Add(sub);
            this.OnAddSubUI(sub);
        }
        protected void RemoveSubUI(JuiAbstract sub)
        {
            this.subUis.Remove(sub);
            this.OnRemoveSubUI(sub);
        }

        protected virtual void Update()
        {
            this.OnUpdate();
            this.subuiUpdateHandler?.Invoke();
            if (this.operateQueue.Count > 0)
            {
                foreach (var item in this.operateQueue)
                {
                    if (item.IsAdd)
                    {
                        this.subuiUpdateHandler += item.Handler;
                    }
                    else
                    {
                        this.subuiUpdateHandler -= item.Handler;
                    }
                }
                this.operateQueue.Clear();
            }
        }

        public virtual void Show()
        {
            if (this.IsShow)
            {
                return;
            }
            _gameObject.SetActive(true);
            this.IsShow = true;

            if (this.parent != null)
            {
                this.parent.PushUIStack(this);
                if (this.attr.EnableUpdate)
                {
                    this.parent.AddUpdateHandler(this.Update);
                }
            }

            this.OnShow();
        }
        /// <summary>
        /// 逻辑上的隐藏，不再成为焦点，IsShow将为false，但OnUpdate还会继续更新
        /// </summary>
        protected virtual void LogicHide()
        {
            this.IsShow = false;

            if (this.parent != null)
            {
                this.parent.PopUIStack(this);
            }

        }
        /// <summary>
        /// 隐藏，OnUpdate将不再更新，且游戏物体取消激活
        /// </summary>
        protected void InactiveHide()
        {
            if (this.parent != null)
            {
                if (this.attr.EnableUpdate)
                {
                    this.parent.RemoveUpdateHandler(this.Update);
                }
            }
            this.OnHide();
            _gameObject.SetActive(false);
        }

        public virtual void Hide()
        {
            if (!this.IsShow)
            {
                return;
            }
            this.LogicHide();
            this.InactiveHide();
        }
        /// <summary>
        /// 创建绑定
        /// </summary>
        protected virtual void CreateBind()
        {
            if (_transform == null && this.parent != null)
            {
                _transform = this.parent.transform.Find(this.attr.Path);
            }
            if (this.attribute.ResourcePath == null)
            {
                string path = this.Name;
                var p = this.Parent;
                if (p != null)
                {
                    path = p.Name + "/" + path;
                }
                this.attribute.ResourcePath = path;
            }
            if (_transform == null)
            {
                var prefab = this.LoadResource(this.attribute.ResourcePath);
                var goinst = UnityEngine.Object.Instantiate(prefab, this.transform);
                goinst.name = this.Name;
                _transform = goinst.transform;
            }

            _gameObject = _transform.gameObject;
            this.IsShow = _gameObject.activeSelf;

            if (this.attr.IsAutoBindElement)
            {
                this.AutoBindElement();
            }
            this.OnInitialize();
        }
        /// <summary>
        /// 初始化UI状态
        /// </summary>
        protected virtual void InitUIState()
        {
            if (this.IsShow && !isInit)
            {
                if (this.parent != null)
                {
                    this.OnShow();
                    this.parent.PushUIStack(this);
                    if (this.attr.EnableUpdate)
                    {
                        this.parent.AddUpdateHandler(this.Update);
                    }
                }

                foreach (var item in this.subUis)
                {
                    item.InitUIState();
                }
                isInit = true;
            }
        }

        public virtual void Destroy()
        {
            if (this.IsShow && this.parent != null)
            {
                this.parent.RemoveSubUI(this);
                if (this.attr.EnableUpdate)
                {
                    this.parent.RemoveUpdateHandler(this.Update);
                }
            }

            this.OnDestroy();

            UnityEngine.Object.Destroy(_gameObject);

            IsShow = false;
            _gameObject = null;
            _transform = null;
            parent = null;
        }

        #region auto binder
        protected static class BindUtil
        {
            public static object GetBindElementObject(Transform tran, Type type)
            {
                if (tran == null)
                {
                    return null;
                }
                if (type == typeof(Transform) || type.IsSubclassOf(typeof(Transform)))
                {
                    return tran;
                }
                else if (type == typeof(Component) || type.IsSubclassOf(typeof(Component)))
                {
                    return tran.GetComponent(type);
                }
                else if (type == typeof(GameObject) || type.IsSubclassOf(typeof(GameObject)))
                {
                    return tran.gameObject;
                }
                return null;
            }
            public static bool IsFieldOrProp(MemberInfo info)
            {
                return info.MemberType == MemberTypes.Field || info.MemberType == MemberTypes.Property;
            }
            public static Type GetFieldOrPropType(MemberInfo info)
            {
                Type type = default;
                if (info.MemberType == MemberTypes.Field)
                {
                    type = ((FieldInfo)info).FieldType;
                }
                else if (info.MemberType == MemberTypes.Property)
                {
                    type = ((PropertyInfo)info).PropertyType;
                }
                return type;
            }
            public static void SetFieldOrPropValue(MemberInfo info, object inst, object value)
            {
                if (info.MemberType == MemberTypes.Field)
                {
                    ((FieldInfo)info).SetValue(inst, value);
                }
                else if (info.MemberType == MemberTypes.Property)
                {
                    ((PropertyInfo)info).SetValue(inst, value);
                }
            }
        }

        private void BindElement(MemberInfo info, Type type)
        {
            JuiElementAttribute attr = info.GetCustomAttribute<JuiElementAttribute>();
            string path = attr.Path != null ? attr.Path : info.Name;

            Transform tran = transform.Find(path);
            if (tran == null)
            {
                Debug.LogWarning(string.Format("JuiElementBinder: {0}.{1} not found.", this.GetType().Name, info.Name));
                return;
            }

            object obj = BindUtil.GetBindElementObject(tran, type);
            BindUtil.SetFieldOrPropValue(info, this, obj);
        }
        private void BindElementArray(MemberInfo info, Type type)
        {
            JuiElementArrayAttribute attr = info.GetCustomAttribute<JuiElementArrayAttribute>();

            string path = attr.Path != null ? attr.Path : info.Name;

            Transform tran = transform.Find(path);
            if (tran == null)
            {
                Debug.LogWarning(string.Format("JuiElementBinder: {0}.{1} not found.", this.GetType().Name, info.Name));
                return;
            }
            object fieldInst = null;

            if (type.IsArray)
            {
                fieldInst = Array.CreateInstance(attr.ElementType, tran.childCount);
                Array arr = (Array)fieldInst;
                for (int i = 0; i < tran.childCount; i++)
                {
                    object inst = BindUtil.GetBindElementObject(tran.GetChild(i), attr.ElementType);
                    arr.SetValue(inst, i);
                }
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                fieldInst = Activator.CreateInstance(type);
                IList list = (IList)fieldInst;
                foreach (Transform childTransform in tran)
                {
                    object inst = BindUtil.GetBindElementObject(childTransform, attr.ElementType);
                    list.Add(inst);
                }
            }
            BindUtil.SetFieldOrPropValue(info, this, fieldInst);
        }
        private void BindElementSubUI(MemberInfo info, Type type)
        {
            var sub = (JuiAbstract)Activator.CreateInstance(BindUtil.GetFieldOrPropType(info), null);
            var subAttr = info.GetCustomAttribute<JuiElementSubPanelAttribute>();
            if (subAttr.Name == null)
            {
                subAttr.Name = info.Name;
            }
            if (subAttr.Path == null)
            {
                subAttr.Path = subAttr.Name;
            }
            sub.parent = this;
            sub.attribute = subAttr;
            BindUtil.SetFieldOrPropValue(info, this, sub);
            this.AddSubUI(sub);
            sub.CreateBind();
        }
        protected virtual void OnBindElement(List<MemberInfo> fields)
        {
            foreach (MemberInfo info in fields)
            {
                if (BindUtil.IsFieldOrProp(info))
                {
                    Type type = BindUtil.GetFieldOrPropType(info);

                    if (info.IsDefined(typeof(JuiElementAttribute)))
                    {
                        BindElement(info, type);
                    }
                    else if (info.IsDefined(typeof(JuiElementArrayAttribute)))
                    {
                        BindElementArray(info, type);
                    }
                    else if (info.IsDefined(typeof(JuiElementSubPanelAttribute)))
                    {
                        BindElementSubUI(info, type);
                    }
                }
            }
        }

        private void AutoBindElement()
        {
            var fields = this.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            List<MemberInfo> infos = new List<MemberInfo>();
            foreach (MemberInfo field in fields)
            {
                if (field.IsDefined(typeof(JuiAbstractAttribute), true))
                {
                    infos.Add(field);
                }
            }
            this.OnBindElement(infos);
        }
        #endregion
        /// <summary>
        /// 在当前面板创建一个子UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="isAutoBindElement"></param>
        /// <param name="enableUpdate"></param>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        protected T CreateSubUI<T>(
            string name = null,
            string path = null,
            bool isAutoBindElement = true,
            bool enableUpdate = false,
            string resourcePath = null) where T : JuiAbstract, new()
        {
            T sub = Activator.CreateInstance<T>();
            var attr = new JuiElementSubPanelAttribute()
            {
                Name = name,
                Path = path,
                IsAutoBindElement = isAutoBindElement,
                EnableUpdate = enableUpdate,
                ResourcePath = resourcePath,
            };

            if (attr.Name == null)
            {
                attr.Name = typeof(T).Name;
            }
            if (attr.Path == null)
            {
                attr.Path = attr.Name;
            }

            this.AddSubUI(sub);

            sub.parent = this;
            sub.attribute = attr;

            sub.CreateBind();
            sub.InitUIState();

            return sub;
        }
        protected void DestroySubUI(JuiAbstract sub)
        {
            this.RemoveSubUI(sub);
            sub.Destroy();
        }
        
        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="path"></param>
        protected virtual GameObject LoadResource(string path)
        {
            return JuiManager.Instance.LoadResource(path);
        }

        /// <summary>
        /// 获取子物体上的组件
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        protected TComponent GetComponentInChild<TComponent>(string path = "") where TComponent : Component
        {
            Transform f = transform;
            if (!string.IsNullOrEmpty(path))
                f = transform.Find(path);
            return f.GetComponent<TComponent>();
        }

        /// <summary>
        /// 显示隐藏切换
        /// </summary>
        public virtual void Switch()
        {
            if (this.IsShow)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }

        public enum MessageType
        {
            Show, Hide, Update, Focus, LostFocus
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="type"></param>
        public void SendMessage(MessageType type)
        {
            switch (type)
            {
                case MessageType.Show: this.OnShow(); break;
                case MessageType.Hide: this.OnHide(); break;
                case MessageType.Update: this.OnUpdate(); break;
                case MessageType.Focus:
                    this.OnFocusSelf();
                    if (this.IsFocus)
                    {
                        this.OnFocus();
                        this.GetSubUIFocus()?.SendMessage(MessageType.Focus);
                    }
                    break;
                case MessageType.LostFocus: 
                    this.OnLostFocusSelf();
                    if(this.IsFocus)
                    {
                        this.OnLostFocus();
                        this.GetSubUIFocus()?.SendMessage(MessageType.LostFocus);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 消息：初始化
        /// </summary>
        protected virtual void OnInitialize() { }
        /// <summary>
        /// 消息：当UI显示时
        /// </summary>
        protected virtual void OnShow() { }
        /// <summary>
        /// 消息：当UI隐藏时
        /// </summary>
        protected virtual void OnHide() { }
        /// <summary>
        /// 消息：当渲染更新时
        /// </summary>
        protected virtual void OnUpdate() { }
        /// <summary>
        /// 消息：当UI销毁时
        /// </summary>
        protected virtual void OnDestroy() { }
        /// <summary>
        /// 消息：获取焦点
        /// </summary>
        protected virtual void OnFocus() { }
        /// <summary>
        /// 消息：失去焦点
        /// </summary>
        protected virtual void OnLostFocus() { }

        /// <summary>
        /// 消息：当有子UI被添加时
        /// </summary>
        /// <param name="sub"></param>
        protected virtual void OnAddSubUI(JuiAbstract sub) { }
        /// <summary>
        /// 消息：当有子UI被移除时
        /// </summary>
        /// <param name="sub"></param>
        protected virtual void OnRemoveSubUI(JuiAbstract sub) { }
        /// <summary>
        /// 消息：作为子UI获取焦点
        /// </summary>
        protected virtual void OnFocusSelf() { }
        /// <summary>
        /// 消息：作为子UI失去焦点
        /// </summary>
        protected virtual void OnLostFocusSelf() { }

        private void AddUpdateHandler(Action act)
        {
            this.operateQueue.Add(new OperateQueue(act, true));
        }
        private void RemoveUpdateHandler(Action act)
        {
            this.operateQueue.Add(new OperateQueue(act, false));
        }
        private void PushUIStack(JuiAbstract sub)
        {
            this.GetSubUIFocus()?.OnLostFocus();
            this.subuiShowStack.Add(sub);
            sub.SendMessage(MessageType.Focus);
        }
        private void PopUIStack(JuiAbstract sub)
        {
            if (sub != this.GetSubUIFocus())
            {
                this.subuiShowStack.Remove(sub);
                return;
            }
            sub.SendMessage(MessageType.LostFocus);
            this.subuiShowStack.Remove(sub);
            this.GetSubUIFocus()?.SendMessage(MessageType.Focus);
        }
        /// <summary>
        /// 获取当前为焦点的子UI
        /// </summary>
        /// <returns></returns>
        public JuiAbstract GetSubUIFocus()
        {
            if (this.subuiShowStack.Count == 0)
            {
                return null;
            }
            return this.subuiShowStack[this.subuiShowStack.Count - 1];
        }
        /// <summary>
        /// 设置某个子UI为焦点
        /// </summary>
        /// <param name="sub"></param>
        protected void SetSubUIFocus(JuiAbstract sub)
        {
            int pos = this.subuiShowStack.IndexOf(sub);
            if (pos < 0)
            {
                return;
            }
            if (sub == this.GetSubUIFocus())
            {
                return;
            }
            //not top
            this.subuiShowStack.Remove(sub);
            this.GetSubUIFocus()?.SendMessage(MessageType.LostFocus);
            this.subuiShowStack.Add(sub);
            sub.SendMessage(MessageType.Focus);
        }
        /// <summary>
        /// 是否有子UI拥有焦点
        /// </summary>
        /// <returns></returns>
        protected bool HasSubUIFocus()
        {
            return this.subuiShowStack.Count != 0;
        }
        /// <summary>
        /// 获取显示的子ui数量
        /// </summary>
        /// <returns></returns>
        protected int GetSubUIShowCount()
        {
            return this.subuiShowStack.Count;
        }
        public List<JuiAbstract> GetSubUIs()
        {
            return this.subUis;
        }
        /// <summary>
        /// 获取子ui数量（包括隐藏的）
        /// </summary>
        /// <returns></returns>
        protected int GetSubUICount()
        {
            return this.subUis.Count;
        }
    }
}