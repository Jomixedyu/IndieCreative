using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JxUnity.Jxugui;

public class JuiInspector : EditorWindow
{
    [MenuItem("Window/UI/JuiInspector")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<JuiInspector>().Show();
    }
    private void OnInspectorUpdate()
    {
        Repaint();
    }
    private void OnGUI()
    {
        if (!JuiManager.HasInstance)
        {
            EditorGUILayout.LabelField("No Instance");
            return;
        }
        EditorGUILayout.LabelField("focus ui: " + JuiManager.Instance.GetFocus()?.Name);
        foreach (var ui in JuiManager.Instance.GetAllUI())
        {
            string name = ui.Key;
            EditorGUILayout.LabelField(name + " : class " + ui.Value.ToString());
            EditorGUILayout.LabelField("\tbinding: " + JuiManager.Instance.HasUIInstance(name).ToString());

            if (JuiManager.Instance.HasUIInstance(name))
            {
                var pui = JuiManager.Instance.GetUIInstance(name);
                EditorGUILayout.LabelField("\tis show: " + pui.IsShow.ToString());
                EditorGUILayout.LabelField("\tsubui focus: " + pui.GetSubUIFocus()?.Name);
                EditorGUILayout.LabelField("\tsubui: ");
                foreach (JuiSubBase subui in pui.GetSubUIs())
                {
                    EditorGUILayout.LabelField("\t\t- " + subui.Name);
                    EditorGUILayout.LabelField("\t\t\tis show: " + subui.IsShow.ToString());
                }
            }

        }
    }
}
