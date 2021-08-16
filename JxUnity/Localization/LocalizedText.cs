using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField]
    private string stringId;
    public string StringId { get => stringId; set => stringId = value; }

    private Text text;

    public string GetText()
    {
        return LocalizationManager.GetString(stringId);
    }

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<Text>();
        }
        if (text == null)
        {
            text = gameObject.AddComponent<Text>();
            //throw new NullReferenceException("text component not found!");
        }
    }

    private void OnEnable()
    {
        text.text = GetText();
        LocalizationManager.LanguageChanged += OnLanguageChange;
    }

    private void OnDisable()
    {
        LocalizationManager.LanguageChanged -= OnLanguageChange;
    }

    //语言变更刷新
    private void OnLanguageChange()
    {
        text.text = GetText();
    }
}
