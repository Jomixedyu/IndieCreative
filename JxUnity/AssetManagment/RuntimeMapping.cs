using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using System.IO;

public class RuntimeMapping : ScriptableObject
{
    [Serializable]
    public class AssetSet
    {
        [SerializeField]
        private List<string> Names = new List<string>();
        [SerializeField]
        private List<string> TypeNames = new List<string>();
        [SerializeField]
        private List<Object> Objects = new List<Object>();

        public void Add(string name, Object obj)
        {
            this.Names.Add(name);
            this.TypeNames.Add(obj.GetType().Name);
            this.Objects.Add(obj);
        }
        public Object Get(string name, Type type)
        {
            for (int i = 0; i < this.Names.Count; i++)
            {
                if(this.Names[i] == name && this.TypeNames[i] == type.Name)
                {
                    return this.Objects[i];
                }
            }

            return null;
        }
    }

    [SerializeField]
    private List<string> Names = new List<string>();
    [SerializeField]
    private List<AssetSet> Assets = new List<AssetSet>();

    private AssetSet GetAssetSet(string name)
    {
        int pos = this.Names.IndexOf(name);
        if (pos < 0)
        {
            return null;
        }
        return this.Assets[pos];
    }

    public void Add(string assetName, Object obj)
    {
        assetName = assetName.ToLower();
        AssetSet set = this.GetAssetSet(assetName);
        if(set == null)
        {
            this.Names.Add(assetName);
            set = new AssetSet();
            this.Assets.Add(set);
        }

        set.Add(Path.GetFileNameWithoutExtension(assetName), obj);
    }

    public Object Get(string name, Type type)
    {
        name = name.ToLower();
        AssetSet set = this.GetAssetSet(name);

        return set.Get(AssetNameUtility.GetShortName(name), type);
    }
}
