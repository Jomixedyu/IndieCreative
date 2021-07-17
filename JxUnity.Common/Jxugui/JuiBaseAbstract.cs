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
            return JuiManager.Instance.GetFocus() == this;
        }
    }
    public override void SetFocus()
    {
        JuiManager.Instance.SetFocus(this);
    }
    private List<JuiSubBase> uiShowStack = new List<JuiSubBase>();
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
        JuiManager.Instance.Push(this);
        if (this.attr.EnableUpdate)
        {
            JuiManager.Instance.AddUpdateHandler(this.Update);
        }
    }
    protected override void LogicHide()
    {
        base.LogicHide();
        JuiManager.Instance.Pop(this);
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
        this.attr = JuiManager.GetUIAttribute(this);

        this.transform = JuiManager.Instance.transform.Find(this.attr.Name);
        base.Create();

        if (this.IsShow)
        {
            JuiManager.Instance.Push(this);
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
        public Action<JuiSubBase> PushUIStack;
        public Action<JuiSubBase> PopUIStack;
        public Func<JuiSubBase> PeekStackTop;
        public Action<JuiSubBase> SetTopStack;
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
    private void PushUIStack(JuiSubBase sub)
    {
        this.PeekStackTop()?.OnLostFocus();
        this.uiShowStack.Add(sub);
        sub.OnFocus();
    }
    private void PopUIStack(JuiSubBase sub)
    {
        if (sub != this.PeekStackTop())
        {
            this.uiShowStack.Remove(sub);
            return;
        }
        sub.OnLostFocus();
        this.uiShowStack.Remove(sub);
        this.PeekStackTop()?.OnFocus();
    }
    private JuiSubBase PeekStackTop()
    {
        if (this.uiShowStack.Count == 0)
        {
            return null;
        }
        return this.uiShowStack[this.uiShowStack.Count - 1];
    }
    private void SetTopStack(JuiSubBase sub)
    {
        int pos = this.uiShowStack.IndexOf(sub);
        if (pos < 0)
        {
            return;
        }
        if (sub == this.PeekStackTop())
        {
            return;
        }
        //not top
        this.uiShowStack.Remove(sub);
        this.PeekStackTop()?.OnLostFocus();
        this.uiShowStack.Add(sub);
        sub.OnFocus();
    }

}

