using JxUnity.Media;
using UnityEngine;

namespace JxUnity.GamePlay.UI
{
    public enum UISoundConfigGroupType
    {
        Enter,
        Exit,
        Down,
        Enable,
        Disable,
    }

    public class UISoundManager : MonoSingleton<UISoundManager>
    {
        private AudioPool audioPool;

        [SerializeField]
        private UISoundConfigAsset config;
        public void SetConfig(UISoundConfigAsset config)
        {
            this.config = config;
        }

        protected override void Awake()
        {
            if (CheckInstanceAndDestroy())
            {
                return;
            }
            base.Awake();
            this.audioPool = GetComponent<AudioPool>();
            if (this.audioPool == null)
            {
                this.audioPool = gameObject.AddComponent<AudioPool>();
            }
        }

        private AudioClip GetClip(string group, UISoundConfigGroupType type)
        {
            foreach (var item in this.config.Groups)
            {
                if (item.GroupName == group)
                {
                    switch (type)
                    {
                        case UISoundConfigGroupType.Enter:
                            return item.Enter;
                        case UISoundConfigGroupType.Exit:
                            return item.Exit;
                        case UISoundConfigGroupType.Down:
                            return item.Down;
                        case UISoundConfigGroupType.Enable:
                            return item.Enable;
                        case UISoundConfigGroupType.Disable:
                            return item.Disable;
                    }
                }
            }
            return null;
        }

        public void Play(AudioClip clip)
        {
            this.audioPool.Play(clip);
        }

        public void PlayByGroup(string group, UISoundConfigGroupType type)
        {
            if (this.config == null)
            {
                var asset = Resources.Load<UISoundConfigAsset>("UISoundConfigAsset");
                this.config = asset;
                if (asset == null)
                {
                    Debug.LogWarning("UISoundManager: not found config");
                    return;
                }
            }

            AudioClip clip = this.GetClip(group, type);
            if (clip != null)
            {
                this.audioPool.Play(clip);
            }

        }
    }
}
