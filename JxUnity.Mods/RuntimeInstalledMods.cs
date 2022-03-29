using System;
using System.Collections.Generic;

namespace JxUnity.Mods
{
    internal class RuntimeInstalledMods
    {
        public List<ModInfo> InstalledMods = new List<ModInfo>();
        public List<string> InstalledModsPath = new List<string>();

        public void Add(ModInfo info, string path)
        {
            var pos = InstalledMods.IndexOf(info);
            if (pos != -1)
            {
                InstalledMods.RemoveAt(pos);
                InstalledModsPath.RemoveAt(pos);
            }
            InstalledMods.Add(info);
            InstalledModsPath.Add(path);
        }
        public void Remove(ModInfo info)
        {
            var pos = InstalledMods.IndexOf(info);
            if (pos != -1)
            {
                InstalledMods.RemoveAt(pos);
                InstalledModsPath.RemoveAt(pos);
            }
        }
        public ModInfo GetInfo(string modId)
        {
            foreach (var item in InstalledMods)
            {
                if (item.Id == modId) return item;
            }
            return null;
        }
        public string GetPath(string modId)
        {
            return GetPath(GetInfo(modId));
        }

        public string GetPath(ModInfo info)
        {
            if (info == null) return null;
            for (int i = 0; i < InstalledMods.Count; i++)
            {
                if (InstalledMods[i].Equals(info))
                {
                    return InstalledModsPath[i];
                }
            }
            return null;
        }
    }
}
