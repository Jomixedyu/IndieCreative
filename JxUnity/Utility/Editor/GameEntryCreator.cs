using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GameEntryCreator
{

    private static string script =
@"using UnityEngine;

public static class Game
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Entry()
    {

    }

    [RuntimeInitializeOnLoadMethod]
    private static void Loaded()
    {

    }


    public static void RequestQuit()
    {
        Quit();
    }

    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}";

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
