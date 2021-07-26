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
    private string GetName(JuiAbstract ui)
    {
        if (ui == null)
        {
            return null;
        }
        return ui.Name + "  { class " + ui.GetType().Name + " }";
    }
    private void OnGUI()
    {
        if (!JuiManager.HasInstance)
        {
            EditorGUILayout.LabelField("No Instance");
            return;
        }
        EditorGUILayout.LabelField("Focus ui: " + JuiManager.Instance.GetFocus()?.Name);
        foreach (var ui in JuiManager.Instance.GetAllUI())
        {
            string name = ui.Key;
            EditorGUILayout.LabelField(GetName(ui.Value));
            EditorGUILayout.LabelField("\tbinding: " + JuiManager.Instance.HasUIInstance(name).ToString());

            if (JuiManager.Instance.HasUIInstance(name))
            {
                var pui = JuiManager.Instance.GetUIInstance(name);
                EditorGUILayout.LabelField("\tis show: " + pui.IsShow.ToString());
                EditorGUILayout.LabelField("\tsubui focus: " + GetName(pui.GetSubUIFocus()));
                EditorGUILayout.LabelField("\tsubui: ");
                foreach (JuiSubBase subui in pui.GetSubUIs())
                {
                    EditorGUILayout.LabelField("\t\t- " + GetName(subui));
                    EditorGUILayout.LabelField("\t\t\tis show: " + subui.IsShow.ToString());
                }
            }

        }
    }
}
