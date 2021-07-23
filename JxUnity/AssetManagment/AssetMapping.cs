using System;
using System.Collections;
using System.Collections.Generic;

public class AssetMapping : IEnumerable<KeyValuePair<string, AssetMapping.MappingItem>>
{
    public class MappingItem
    {
        public string assetPath { get; private set; }
        public string assetName { get; private set; }
        public string guid { get; private set; }
        public string assetPackageName { get; private set; }

        public MappingItem(string assetPath, string assetName, string guid, string assetBundleName)
        {
            this.assetPath = assetPath;
            this.assetName = assetName;
            this.guid = guid;
            this.assetPackageName = assetBundleName;
        }
    }

    private Dictionary<string, MappingItem> mappingTable;

    public AssetMapping(string content)
    {
        this.mappingTable = new Dictionary<string, MappingItem>();

        string[] mappingLines = content.Replace("\r\n", "\n").Split('\n');
        foreach (string item in mappingLines)
        {
            if (string.IsNullOrEmpty(item))
            {
                continue;
            }
            string[] relation = item.Trim().Split(',');
            var assetPath = relation[0];
            var assetName = relation[1];
            var guid = relation[2];
            var abName = relation[3];
            this.mappingTable.Add(assetPath, new MappingItem(assetPath, assetName, guid, abName));
        }
    }

    public MappingItem Mapping(string assetPath)
    {
        MappingItem item = null;
        if (this.mappingTable.TryGetValue(assetPath, out item))
        {
            return item;
        }
        return null;
    }

    public IEnumerator<KeyValuePair<string, MappingItem>> GetEnumerator()
    {
        return this.mappingTable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.mappingTable.GetEnumerator();
    }
}
