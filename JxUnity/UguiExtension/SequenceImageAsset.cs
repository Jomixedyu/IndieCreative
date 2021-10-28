using System;
using System.Collections.Generic;
using UnityEngine;

namespace JxUnity.Ugui
{
    [CreateAssetMenu(menuName = "JxUgui/Sequence Image Asset")]
    public class SequenceImageAsset : ScriptableObject
    {
        [SerializeField]
        public Sprite[] Images;
        [SerializeField]
        public float Speed = 1;
        [SerializeField]
        public float FrameRate = 15;

        public int Count()
        {
            return this.Images.Length;
        }

        public Sprite this[int i]
        {
            get
            {
                return this.Images[i];
            }
        }
    }
}
