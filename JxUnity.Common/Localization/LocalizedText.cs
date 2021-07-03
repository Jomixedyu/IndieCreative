using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [SerializeField]
    private string stringChunk;
    public string StringChunk => stringChunk;
    
    [SerializeField] 
    private string stringId;
    public string StringId => stringId;

    [SerializeField] 
    private Text text;

    public void SetLocalizedId(string chunkName, string id)
    {
        this.stringChunk = chunkName;
        this.stringId = id;
    }

    public string GetText()
    {
        return LocalizationManager.Instance.GetString(stringChunk, stringId);
    }

    private void Awake()
    {
        if (text == null) 
            text = GetComponent<Text>();
        if (text == null)
            throw new NullReferenceException("Text组件不存在！");
    }

    private void OnEnable()
    {
        text.text = GetText();
        LocalizationManager.Instance.LanguageChanged += OnLanguageChange;
    }

    private void OnDisable()
    {
        LocalizationManager.Instance.LanguageChanged -= OnLanguageChange;
    }

    //语言变更刷新
    private void OnLanguageChange()
    {
        text.text = GetText();
    }
}
