using System;
using UnityEngine;

namespace JxUnity.Jxugui
{
    public abstract class JuiBaseAbstract : JuiAbstract, IDisposable
    {
        public override JuiAbstract Parent
        {
            get => null;
            set => throw new NotSupportedException("only subui is supported.");
        }

        public override bool IsFocusSelf => JuiManager.Instance.GetFocus() == this;
        public override bool IsFocus => this.IsFocusSelf;

        public override void SetFocus()
        {
            JuiManager.Instance.SetFocus(this);
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
        protected override void CreateBind()
        {
            this.attr = JuiManager.GetUIAttribute(this.GetType());

            this.transform = JuiManager.Instance.transform.Find(this.attr.Name);
            base.CreateBind();

            string uiName = this.attr.Name;

            if (!JuiManager.Instance.Exist(uiName))
            {
                JuiManager.Instance.RegisterUI(uiName);
            }
            if (!JuiManager.Instance.HasUIInstance(uiName))
            {
                JuiManager.Instance.SetUIInstance(uiName, this);
            }
        }

        protected override void InitUIState()
        {
            if (this.IsShow)
            {
                this.OnShow();
                JuiManager.Instance.Push(this);
                if (this.attr.EnableUpdate)
                {
                    JuiManager.Instance.AddUpdateHandler(this.Update);
                }
            }
            base.InitUIState();
        }


        protected override GameObject LoadResource(string path)
        {
            return JuiManager.Instance.LoadResource(path);
        }

        public override void Destroy()
        {
            if (this.IsShow)
            {
                JuiManager.Instance.Pop(this);
                if (this.attr.EnableUpdate)
                {
                    JuiManager.Instance.RemoveUpdateHandler(this.Update);
                }
            }

            base.Destroy();
        }

        public virtual void Dispose()
        {
            this.Destroy();
        }
    }

}