using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JxUnity.Ugui
{
    [RequireComponent(typeof(Image))]
    public class SequenceImage : MonoBehaviour
    {
        private Image image = null;

        [SerializeField]
        private SequenceImageAsset images;
        public SequenceImageAsset ImageAsset { get => images; set => images = value; }

        public float FrameRate { get => images.FrameRate; }
        public float Speed { get => images.Speed; }

        [SerializeField]
        private bool isPlaying = false;
        public bool IsPlaying { get => isPlaying; }

        [SerializeField]
        private int index = 0;
        public int Index { get => index; }

        [SerializeField]
        private bool playOnAwake = false;
        public bool PlayOnAwake { get => PlayOnAwake; }

        private void Awake()
        {
            this.image = this.GetComponent<Image>();
            if (this.image == null)
            {
                this.image = this.gameObject.AddComponent<Image>();
            }
            if (this.playOnAwake) this.Play();
        }

        public void Play()
        {
            this.isPlaying = true;
            var rect = this.GetComponent<RectTransform>();
            var targetRect = this.images[0].rect;
            rect.sizeDelta = new Vector2(targetRect.width, targetRect.height);
        }
        public void Stop()
        {
            this.isPlaying = false;
        }

        private float count = 0;
        private void Update()
        {
            if (this.isPlaying)
            {
                if (this.images == null || this.images.Count() == 0)
                {
                    return;
                }
                this.count += Time.deltaTime;
                float frameTime = this.Speed / this.FrameRate;
                while (this.count >= frameTime)
                {
                    this.index++;
                    if (this.images.Count() == this.index) this.index = 0;
                    this.count -= frameTime;
                }
                this.image.sprite = this.images[this.index];
            }
        }

    }

}

