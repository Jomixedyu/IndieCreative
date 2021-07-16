using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JuiInspector : EditorWindow
{
    [MenuItem("Window/UI/JuiInspector")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<JuiInspector>().Show();
    }

    private void OnGUI()
    {
        if (!JuiManager.HasInstance)
        {
            EditorGUILayout.LabelField("No Instance");
            return;
        }

        foreach (string ui in JuiManager.Instance.GetAllUI())
        {
            if (JuiManager.Instance.HasUIInstance(ui))
            {
                EditorGUILayout.LabelField(ui + "\tbinding");
            }
            else
            {
                EditorGUILayout.LabelField(ui + "\tno binding");
            }
        }
    }
}
