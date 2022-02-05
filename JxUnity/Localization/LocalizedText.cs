using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JxUnity.Localization
{
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField]
        private string stringId;
        public string StringId { get => stringId; set => stringId = value; }

        private Text txt;
        
        public string GetText()
        {
            return LocalizationManager.GetString(this.stringId);
        }
        public void RefreshText()
        {
            var str = this.GetText();
            if (str != null)
            {
                this.txt.text = str;
            }
        }

        private void Awake()
        {
            if (this.txt == null)
            {
                this.txt = this.GetComponent<Text>();
                if (this.txt == null)
                {
                    this.txt = this.gameObject.AddComponent<Text>();
                }
            }

        }

        private void OnEnable()
        {
            this.RefreshText();
            LocalizationManager.LanguageChanged += this.OnLanguageChange;
        }

        private void OnDisable()
        {
            LocalizationManager.LanguageChanged -= this.OnLanguageChange;
        }

        //语言变更刷新
        private void OnLanguageChange()
        {
            this.RefreshText();
        }
    }
}
