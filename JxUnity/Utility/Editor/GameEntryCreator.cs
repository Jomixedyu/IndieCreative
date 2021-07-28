using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GameEntryCreator
{

    private static string script =
        "using UnityEngine;\n" +
        "\n" +
        "public static class GameEntry\n" +
        "{\n" +
        "    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]\n" +
        "    private static void Entry()\n" +
        "    {\n" +
        "        \n" +
        "    }\n" +
        "\n" +
        "    [RuntimeInitializeOnLoadMethod]\n" +
        "    private static void Loaded()\n" +
        "    {\n" +
        "        \n" +
        "    }\n" +
        "}";

    [MenuItem("Assets/Create/GameEntry", validate = true)]
    public static bool ValidCreate()
    {
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return AssetDatabase.IsValidFolder(path);
    }

    [MenuItem("Assets/Create/GameEntry")]
    public static void Create()
    {
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!AssetDatabase.IsValidFolder(path))
        {
            return;
        }
        path += "/GameEntry.cs";
        if (File.Exists(path))
        {
            Debug.Log("GameEntry.cs exists.");
            return;
        }
        File.WriteAllText(path, script);
        AssetDatabase.Refresh();
    }

}
