using System;
using UnityEngine;

namespace JxUnity.Utility
{
    public static class CursorController
    {
        public static void EnableAutoHide(float waitTime)
        {
            CursorControllerMono.Instance.enabled = true;
            CursorControllerMono.Instance.waitTime = waitTime;
        }
        public static void DisableAutoHide()
        {
            if (CursorControllerMono.HasInstance)
            {
                CursorControllerMono.Instance.enabled = false;
            }
        }
    }

    internal class CursorControllerMono : MonoBehaviour
    {
        private static CursorControllerMono instance;
        public static CursorControllerMono Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject gameObject = new GameObject($"__m_{nameof(CursorControllerMono)}");
                    DontDestroyOnLoad(gameObject);
                    instance = gameObject.AddComponent<CursorControllerMono>();
                }
                return instance;
            }
        }
        public static bool HasInstance => instance != null;

        internal float waitTime = 5;

        private float now = 0;

        private Vector3 lastPosition;

        private void OnDisable()
        {
            Cursor.visible = true;
        }

        private void Update()
        {
            if (Input.mousePosition == lastPosition)
            {
                if (Cursor.visible)
                {
                    if (now >= waitTime)
                    {
                        now = 0;
                        Cursor.visible = false;
                    }
                    else
                    {
                        now += Time.unscaledDeltaTime;
                    }
                }
            }
            else
            {
                Cursor.visible = true;
                now = 0;
            }

            lastPosition = Input.mousePosition;
        }
    }
}
