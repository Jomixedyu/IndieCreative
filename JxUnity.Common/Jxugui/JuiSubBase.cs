using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class JuiSubBase : JuiAbstract
{
    private JuiBaseAbstract parentUI;
    public JuiBaseAbstract ParentUI { get => parentUI; }
    private JuiBaseAbstract.JuiBaseAbstractPack pack;

    /// <summary>
    /// 当前子面板在父面板中的是否为焦点
    /// </summary>
    public bool IsFocusSelf
    {
        get
        {
            return this.pack.PeekStackTop() == this;
        }
    }

    public override bool IsFocus
    {
        get
        {
            return this.parentUI.IsFocus && this.IsFocusSelf;
        }
    }
    public override void SetFocus()
    {
        this.pack.SetTopStack(this);
    }

    public void InitializeUI(
        JuiBaseAbstract parentUI,
        JuiElementSubPanelAttribute attr,
        JuiBaseAbstract.JuiBaseAbstractPack pack)
    {
        this.parentUI = parentUI;
        this.pack = pack;

        //init attribute
        this.attr = attr;
        if (attr.Name == null)
        {
            attr.Name = this.GetType().Name;
        }
        if (attr.Path == null)
        {
            attr.Path = attr.Name;
        }
    }
    public override void Create()
    {
        var attr = this.attr as JuiElementSubPanelAttribute;
        //init transform
        this.transform = this.parentUI.transform.Find(attr.Path);
        if (this.transform == null)
        {
            Debug.LogWarning(string.Format(
                "not found ui: {0}/{1}",
                this.parentUI.Name,
                this.attr.Name));
        }

        base.Create();

        if (this.IsShow)
        {
            this.pack.PushUIStack(this);
            if (this.attr.EnableUpdate)
            {
                this.pack.AddUpdateHandler(this.Update);
            }
        }
    }

    public override void Show()
    {
        if (this.IsShow)
        {
            return;
        }
        base.Show();
        this.pack.PushUIStack(this);
        if (this.attr.EnableUpdate)
        {
            this.pack.AddUpdateHandler(this.Update);
        }
    }
    protected override void LogicHide()
    {
        base.LogicHide();
        this.pack.PopUIStack(this);
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
            this.pack.RemoveUpdateHandler(this.Update);
        }
    }

    protected override GameObject LoadResource(string path)
    {
        return JuiManager.Instance.LoadResource(this.parentUI.Name + '/' + path);
    }
}
