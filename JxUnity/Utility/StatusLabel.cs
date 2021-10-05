using System;
using System.Collections.Generic;
using UnityEngine;

namespace JxUnity.Utility
{
    public interface IStatusLabel
    {
        void OnUpdate(float udt);
        string OnRenderText();
    }

    public class StatusLabelFps : IStatusLabel
    {
        private float time;
        private int frameCount;
        private string showText;

        public void OnUpdate(float udt)
        {
            this.time += udt;
            this.frameCount++;
            if (this.time >= 1 && this.frameCount >= 1)
            {
                int fps = this.frameCount / (int)time;
                this.time = 0;
                this.frameCount = 0;
                this.showText = "fps:" + fps.ToString();
            }
        }

        public string OnRenderText()
        {
            return this.showText;
        }
    }


    internal class StatusLabel : MonoBehaviour
    {
        private static StatusLabel instance;
        private static StatusLabel Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject($"__m_{nameof(StatusLabel)}");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<StatusLabel>();
                }
                return instance;
            }
        }

        private IList<IStatusLabel> status;

        private void Awake()
        {
            this.status = new List<IStatusLabel>();
        }

        public static void AddLabel(IStatusLabel statusLabel)
        {
            var self = Instance;
            if (self.status.Contains(statusLabel))
            {
                return;
            }
            self.status.Add(statusLabel);
        }
        private void Update()
        {
            foreach (var item in this.status)
            {
                item.OnUpdate(Time.unscaledDeltaTime);
            }
        }
        private void OnGUI()
        {
            foreach (var item in this.status)
            {
                string txt = item.OnRenderText();
                if (txt == null)
                {
                    continue;
                }
                float width = txt.Length * 8;
                GUI.Label(new Rect(0, 0, width, 20), txt);
            }

        }
    }

}