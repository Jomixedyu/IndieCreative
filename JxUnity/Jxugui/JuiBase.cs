using System;
using UnityEngine;

namespace JxUnity.Jxugui
{
    /// <summary>
    /// 顶级UI
    /// </summary>
    /// <typeparam name="UIType"></typeparam>
    public abstract class JuiBase<UIType> : JuiBaseAbstract, IDisposable where UIType : JuiBase<UIType>, new()
    {
        private static UIType mInstance;
        public static UIType Instance
        {
            get => GetInstance();
        }
        public static bool HasInstance
        {
            get => mInstance != null;
        }
        public static UIType GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = Activator.CreateInstance<UIType>();
                mInstance.CreateBind();
                mInstance.InitUIState();
            }
            return mInstance;
        }
        private static void ResetInstance()
        {
            mInstance = Activator.CreateInstance<UIType>();
            mInstance.CreateBind();
            mInstance.InitUIState();
        }
        public override void Dispose()
        {
            base.Dispose();
            mInstance = null;
        }

    }
}