using JxUnity.ResDB;
using JxUnity.ResDB.Private;
using UnityEditor;
using UnityEngine;

public static class AssetSettingsProvider
{
    
    private static readonly string ConfigPath = $"Assets/Resources/{ResDBConfig.kSettingObjectName}.asset";

    [SettingsProvider]
    public static SettingsProvider GetProvider()
    {
        return new Provider();
    }

    public static AssetLoadMode GetDefaultLoadMode()
    {
        var set = AssetDatabase.LoadAssetAtPath<ResDBSettings>(ConfigPath);
        return set != null ? set.LoadMode : AssetLoadMode.Inline;
    }
    public static void SetDefaultLoadMode(AssetLoadMode mode)
    {
        var set = AssetDatabase.LoadAssetAtPath<ResDBSettings>(ConfigPath);
        if (set == null)
        {
            set = ScriptableObject.CreateInstance<ResDBSettings>();
            AssetDatabase.CreateAsset(set, ConfigPath);
            AssetDatabase.Refresh();
        }
        set.LoadMode = mode;
        EditorUtility.SetDirty(set);
        //AssetDatabase.SaveAssets();
    }


    public class Provider : SettingsProvider
    {
        private SerializedObject serobj;
        private SerializedProperty ser_Mode;

        public Provider()
            : base("Project/JxUnity.ResDB", SettingsScope.Project, null)
        {
        }
        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);
            EditorGUILayout.PropertyField(this.ser_Mode);
            this.serobj.ApplyModifiedProperties();
        }

        public override void OnActivate(string searchContext, UnityEngine.UIElements.VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            var assetObj = AssetDatabase.LoadAssetAtPath<ResDBSettings>(ConfigPath);
            if (assetObj == null)
            {
                assetObj = ScriptableObject.CreateInstance<ResDBSettings>();
                AssetDatabase.CreateAsset(assetObj, ConfigPath);
                AssetDatabase.Refresh();
            }

            this.serobj = new SerializedObject(assetObj);
            this.ser_Mode = this.serobj.FindProperty("LoadMode");
        }
    }
}

