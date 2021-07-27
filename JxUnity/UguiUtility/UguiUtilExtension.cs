using System;
using UnityEngine;
using UnityEngine.UI;

public static class UguiUtilExtension
{
    /// <summary>
    /// 设置按钮文本
    /// </summary>
    /// <param name="button"></param>
    /// <param name="text"></param>
    public static void SetUButtonText(this Button button, string text)
    {
        button.transform.Find("Text").GetComponent<Text>().text = text;
    }
    /// <summary>
    /// 从一级子物体中查找并获取组件
    /// </summary>
    /// <param name="self"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static Text GetChildUText(this Transform self, string childName = "Text")
    {
        return self.Find(childName).GetComponent<Text>();
    }
    /// <summary>
    /// 从一级子物体中查找并获取组件
    /// </summary>
    /// <param name="self"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static Button GetChildUButton(this Transform self, string childName = "Button")
    {
        return self.Find(childName).GetComponent<Button>();
    }
    /// <summary>
    /// 从一级子物体中查找并获取组件
    /// </summary>
    /// <param name="self"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static Image GetChildUImage(this Transform self, string childName = "Image")
    {
        return self.Find(childName).GetComponent<Image>();
    }
}
