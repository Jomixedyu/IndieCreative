using JxUnity.Resources;
using UnityEditor;
using UnityEngine;

public static class AssetSettingsProvider
{
    private const string ConfigPath = "Assets/Resources/ResourcePackageSettings.asset";

    [SettingsProvider]
    public static SettingsProvider GetProvider()
    {
        return new Provider();
    }

    public static AssetLoadMode GetDefaultLoadMode()
    {
        var set = AssetDatabase.LoadAssetAtPath<ResourcePackageSettings>(ConfigPath);
        return set != null ? set.DefaultLoadMode : AssetLoadMode.Local;
    }
    public static void SetDefaultLoadMode(AssetLoadMode mode)
    {
        var set = AssetDatabase.LoadAssetAtPath<ResourcePackageSettings>(ConfigPath);
        if (set == null)
        {
            set = ScriptableObject.CreateInstance<ResourcePackageSettings>();
            AssetDatabase.CreateAsset(set, ConfigPath);
            AssetDatabase.Refresh();
        }
        set.DefaultLoadMode = mode;
        EditorUtility.SetDirty(set);
        //AssetDatabase.SaveAssets();
    }


    public class Provider : SettingsProvider
    {
        private SerializedObject serobj;
        private SerializedProperty ser_Mode;

        public Provider()
            : base("Project/Resource Package", SettingsScope.Project, null)
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
            var assetObj = AssetDatabase.LoadAssetAtPath<ResourcePackageSettings>(ConfigPath);
            if (assetObj == null)
            {
                assetObj = ScriptableObject.CreateInstance<ResourcePackageSettings>();
                AssetDatabase.CreateAsset(assetObj, ConfigPath);
                AssetDatabase.Refresh();
            }

            this.serobj = new SerializedObject(assetObj);
            this.ser_Mode = this.serobj.FindProperty("DefaultLoadMode");
        }
    }
}

