using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JxUnity.Pool;

public class GameObjectPoolInspector : EditorWindow
{
    [MenuItem("Window/Analysis/GameObjectPoolInspector")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<GameObjectPoolInspector>().Show();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        if (!GameObjectPool.IsEnabled())
        {
            return;
        }
        GameObjectPool.ForEach(item =>
        {
            EditorGUILayout.LabelField($"{item.Type}: {item.UsableCount}/{item.ObjectCount}");
        });
    }
}
