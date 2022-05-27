using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ProjectBroswer
{
    public static void ShowFolderContents(string path)
    {
        var ass = Assembly.Load("UnityEditor");
        var type = ass.GetType("UnityEditor.ProjectBrowser");
        var win = EditorWindow.GetWindow(type);
        win.Show();

        Selection.activeObject = null;

        int instId = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path).GetInstanceID();
        type.GetMethod("ShowFolderContents",
            BindingFlags.NonPublic | BindingFlags.Instance, null,
            new System.Type[] { typeof(int), typeof(bool) }, null)
            .Invoke(win, new object[] { instId, true });
    }
}