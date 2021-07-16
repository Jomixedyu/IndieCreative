using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class JuiBaseAbstract : JuiAbstract
{
    public override bool IsFocus
    {
        get
        {
            return JuiManager.Instance.GetFocus() == this.GetType();
        }
    }
    public void SetFocus()
    {
        JuiManager.Instance.SetFocus(this.GetType());
    }
    private List<Type> uiShowStack = new List<Type>();
    private Action updateHandler;
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

    protected override void Update()
    {
        base.Update();
        this.updateHandler?.Invoke();
        if (this.operateQueue.Count > 0)
        {
            foreach (var item in this.operateQueue)
            {
                if (item.IsAdd)
                {
                    this.updateHandler += item.Handler;
                }
                else
                {
                    this.updateHandler -= item.Handler;
                }
            }
            this.operateQueue.Clear();
        }
    }
    public override void Show()
    {
        if (this.IsShow)
        {
            return;
        }
        base.Show();
        JuiManager.Instance.Push(this.GetType());
        if (this.attr.EnableUpdate)
        {
            JuiManager.Instance.AddUpdateHandler(this.Update);
        }
    }
    protected override void LogicHide()
    {
        base.LogicHide();
        JuiManager.Instance.Pop(this.GetType());
    }
    public override void Hide()
    {
        if (!this.IsShow)
        {
            return;
        }
        base.Hide();
        if (this.attr.EnableUpdate)
        {
            JuiManager.Instance.RemoveUpdateHandler(this.Update);
        }
    }
    public override void Create()
    {
        this.attr = this.GetType().GetCustomAttribute<JuiPanelAttribute>(false);
        if (this.attr.Name == null)
        {
            this.attr.Name = this.GetType().Name;
        }

        this.transform = JuiManager.Instance.transform.Find(this.attr.Name);
        base.Create();

        if (this.IsShow)
        {
            JuiManager.Instance.Push(this.GetType());
            if (this.attr.EnableUpdate)
            {
                JuiManager.Instance.AddUpdateHandler(this.Update);
            }
        }

        string uiName = this.GetType().Name;

        if (!JuiManager.Instance.Exist(uiName))
        {
            JuiManager.Instance.RegisterUI(uiName);
        }
        if (!JuiManager.Instance.HasUIInstance(uiName))
        {
            JuiManager.Instance.SetUI(uiName, this);
        }
        //TODO: 绑定子UI
    }

    protected override GameObject LoadResource(string path)
    {
        return JuiManager.Instance.LoadResource(path);
    }

    protected override void OnBindElement(List<FieldInfo> fields)
    {
        base.OnBindElement(fields);
        foreach (FieldInfo field in fields)
        {
            if (field.IsDefined(typeof(JuiElementSubPanelAttribute)))
            {
                var sub = (JuiSubBase)Activator.CreateInstance(field.FieldType, null);
                var subAttr = field.GetCustomAttribute<JuiElementSubPanelAttribute>();
                if (subAttr.Name == null)
                {
                    subAttr.Name = field.Name;
                }
                sub.InitializeUI(this, subAttr, this.GetSubUiIniter());
                field.SetValue(this, sub);
                sub.Create();
            }
        }
    }

    public class JuiBaseAbstractPack
    {
        public JuiBaseAbstract Ui;
        public Action<Type> PushUIStack;
        public Action<Type> PopUIStack;
        public Func<Type> PeekStackTop;
        public Action<Type> SetTopStack;
        public Action<Action> AddUpdateHandler;
        public Action<Action> RemoveUpdateHandler;
    }
    private JuiBaseAbstractPack pack;
    private JuiBaseAbstractPack GetSubUiIniter()
    {
        if (this.pack == null)
        {
            this.pack = new JuiBaseAbstractPack()
            {
                Ui = this,
                PushUIStack = this.PushUIStack,
                PopUIStack = this.PopUIStack,
                PeekStackTop = this.PeekStackTop,
                SetTopStack = this.SetTopStack,
                AddUpdateHandler = this.AddUpdateHandler,
                RemoveUpdateHandler = this.RemoveUpdateHandler
            };
        }
        return this.pack;
    }
    private void AddUpdateHandler(Action act)
    {
        this.operateQueue.Add(new OperateQueue(act, true));
    }
    private void RemoveUpdateHandler(Action act)
    {
        this.operateQueue.Add(new OperateQueue(act, false));
    }
    private void PushUIStack(Type sub)
    {
        this.uiShowStack.Add(sub);
    }
    private void PopUIStack(Type subBase)
    {
        this.uiShowStack.Remove(subBase);
    }
    private Type PeekStackTop()
    {
        if (this.uiShowStack.Count == 0)
        {
            return null;
        }
        return this.uiShowStack[this.uiShowStack.Count - 1];
    }
    private void SetTopStack(Type subBase)
    {
        int pos = this.uiShowStack.IndexOf(subBase);
        if (pos > -1)
        {
            this.uiShowStack.Remove(subBase);
            this.uiShowStack.Add(subBase);
        }
    }

}

