using System;
using UnityEngine;

namespace JxUnity.GamePlay.UI
{
    [Serializable]
    public class UISoundConfigGroup
    {
        public string GroupName;
        public AudioClip Enter;
        public AudioClip Exit;
        public AudioClip Down;
        public AudioClip Enable;
        public AudioClip Disable;
    }

    [Serializable]
    [CreateAssetMenu(fileName = "UISoundConfigAsset", menuName = "GamePlay/UI/UISoundConfigAsset")]
    public class UISoundConfigAsset : ScriptableObject
    {
        [SerializeField]
        public UISoundConfigGroup[] Groups;
    }
}
