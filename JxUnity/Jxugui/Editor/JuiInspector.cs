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

    private Dictionary<int, string> levelTabCache = new Dictionary<int, string>()
    {
        {0, "" },
        {1, "\t" },
        {2, "\t\t" },
        {3, "\t\t\t" },
        {4, "\t\t\t\t" },
        {5, "\t\t\t\t\t" },
        {6, "\t\t\t\t\t\t" },
        {7, "\t\t\t\t\t\t\t" },
        {8, "\t\t\t\t\t\t\t\t" },
    };

    private string GetLevelTab(int level)
    {
        string value;
        if (!levelTabCache.TryGetValue(level, out value))
        {
            for (int i = 0; i < level; i++)
            {
                value += "\t";
            }
        }
        return value;
    }

    private void ShowSubUI(JuiAbstract pui, int level = 0)
    {
        if (pui.GetSubUICount() == 0)
        {
            return;
        }

        foreach (JuiSubBase subui in pui.GetSubUIs())
        {
            EditorGUILayout.LabelField(GetLevelTab(level + 2) + "- " + GetName(subui));
            EditorGUILayout.LabelField(GetLevelTab(level + 3) + "is show: " + subui.IsShow.ToString());
            var _sub = subui.GetSubUIFocus();
            if (_sub != null)
            {
                EditorGUILayout.LabelField(GetLevelTab(level + 3) + "subui focus: " + _sub.Name);
                ShowSubUI(subui, level + 1);
            }
        }
    }

    private Vector2 scrollView;

    private void OnGUI()
    {

        if (!JuiManager.HasInstance)
        {
            EditorGUILayout.LabelField("No Instance");
            return;
        }

        this.scrollView = EditorGUILayout.BeginScrollView(this.scrollView);
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
                this.ShowSubUI(pui);
                foreach (JuiSubBase subui in pui.GetSubUIs())
                {
                    EditorGUILayout.LabelField("\t\t- " + GetName(subui));
                    EditorGUILayout.LabelField("\t\t\tis show: " + subui.IsShow.ToString());
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
