using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JxUnity.Utility
{
    public interface IStatusLabel
    {
        void OnUpdate(float udt);
        string OnRenderText();
    }

    /// <summary>
    /// 每秒网络延迟标签
    /// </summary>
    public class StatusLabelNetDelay : IStatusLabel
    {
        private Func<float> getter;
        public StatusLabelNetDelay(Func<float> getter)
        {
            this.getter = getter;
        }
        public string OnRenderText()
        {
            return this.getter.Invoke() + "ms";
        }

        public void OnUpdate(float udt)
        {

        }
    }

    /// <summary>
    /// 纯文本
    /// </summary>
    public class StatusLabelText : IStatusLabel
    {
        private Func<string> getter;
        private string text;
        /// <summary>
        /// 动态文本
        /// </summary>
        /// <param name="getter"></param>
        public StatusLabelText(Func<string> getter)
        {
            this.getter = getter;
        }

        /// <summary>
        /// 静态文本
        /// </summary>
        /// <param name="text"></param>
        public StatusLabelText(string text)
        {
            this.text = text;
        }
        public string OnRenderText()
        {
            return this.getter != null ? this.getter.Invoke() : this.text;
        }

        public void OnUpdate(float udt) { }
    }
    /// <summary>
    /// 每秒帧率标签
    /// </summary>
    public class StatusLabelFps : IStatusLabel
    {
        private float time;
        private int frameCount;
        private string showText = "fps: 0";

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
    public class StatusLabelBattery : IStatusLabel
    {
        public string OnRenderText()
        {
            return "battery: "+ (SystemInfo.batteryLevel * 100).ToString() + "%";
        }

        public void OnUpdate(float udt)
        {
        }
    }

    public class StatusLabel : MonoBehaviour
    {
        public enum PositionType
        {
            UpperLeft,
            UpperRight,
            LowerLeft,
            LowerRight,
        }

        private static StatusLabel instance;
        public static StatusLabel Instance
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

        public Vector2Int offset { get; set; }
        public static Vector2Int Offset { get => Instance.offset; set => Instance.offset = value; }

        private IList<IStatusLabel> status;
        private StringBuilder sb;
        private string text;

        private GUIStyle style;

        private PositionType _positionType;
        public PositionType positionType
        {
            get => this._positionType;
            set
            {
                this._positionType = value;
                switch (value)
                {
                    case PositionType.UpperLeft:
                        this.style.alignment = TextAnchor.UpperLeft;
                        break;
                    case PositionType.UpperRight:
                        this.style.alignment = TextAnchor.UpperRight;
                        break;
                    case PositionType.LowerLeft:
                        this.style.alignment = TextAnchor.LowerLeft;
                        break;
                    case PositionType.LowerRight:
                        this.style.alignment = TextAnchor.LowerRight;
                        break;
                    default:
                        break;
                }
            }
        }

        public static PositionType Position { get => Instance.positionType; set => Instance.positionType = value; }

        private void Awake()
        {
            this.status = new List<IStatusLabel>();
            this.sb = new StringBuilder();
            this.text = string.Empty;

            this.style = new GUIStyle();
            this.style.normal.textColor = Color.white;

            this.positionType = PositionType.UpperLeft;
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

        float dt = 0;
        private void Update()
        {
            float udt = Time.unscaledDeltaTime;
            foreach (var item in this.status)
            {
                item.OnUpdate(udt);
            }
            this.dt += udt;
        }

        private void OnGUI()
        {
            if (this.dt > 1f)
            {
                this.dt = 0f;
                int count = this.status.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = this.status[i];
                    sb.Append(item.OnRenderText());
                    if (i != count - 1) //no last
                    {
                        sb.Append("    ");
                    }
                }
                this.text = sb.ToString();
                sb.Clear();
            }

            int width = Screen.width - offset.x * 2;
            int height = Screen.height - offset.y * 2;
            GUI.Label(new Rect(offset.x, offset.y, width, height), this.text, style);

        }
    }

}