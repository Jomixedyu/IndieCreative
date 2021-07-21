using System;
using UnityEngine;
using UnityEngine.UI;

public static class UguiUtilExtension
{
    public static void SetUButtonText(this Button button, string text)
    {
        button.transform.Find("Text").GetComponent<Text>().text = text;
    }
    public static void SetUImageSprite(this Transform transform, Sprite sprite)
    {
        transform.GetComponent<Image>().sprite = sprite;
    }
}
