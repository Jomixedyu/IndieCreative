using JxUnity.Mods.UserInterface;
using JxUnity.Mods.UserInterfaceImpl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    public class ModManager
    {
        private static RuntimeInstalledMods installedMods;

        public static IReadOnlyList<ModInfo> InstalledMods { get => installedMods.InstalledMods; }

        private static List<ModObject> loadedMods;
        public static IReadOnlyList<ModObject> LoadedMods { get => loadedMods; }

        private static ModEventHub modEventHub;
        public static ModEventHub ModEventHub { get => modEventHub; }

        public static ISystem System { get; private set; } = new SystemImpl();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Init()
        {
            installedMods = new RuntimeInstalledMods();
            loadedMods = new List<ModObject>();
            modEventHub = new ModEventHub();
        }

        /// <summary>
        /// 配置文件读取进内存
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ModInfo InstallMod(string path)
        {
            var info = ModLoader.LoadInfo(path + "/config.xml");
            if (info == null)
            {
                return null;
            }
            installedMods.Add(info, path);
            return info;
        }

        /// <summary>
        /// 把全部配置文件读取进内存
        /// </summary>
        /// <param name="folderPath"></param>
        public static void BatchInstallModFromFolder(string folderPath)
        {
            var dirs = Directory.GetDirectories(folderPath);
            foreach (var dir in dirs)
            {
                InstallMod(dir);
            }
        }


        public static ModInfo GetModInfo(string modId)
        {
            return installedMods.GetInfo(modId);
        }
        public static ModObject GetLoadedModObject(string modId)
        {
            foreach (var item in loadedMods)
            {
                if (item.Info.Id == modId)
                {
                    return item;
                }
            }
            return null;
        }

        public static bool IsLoadedMod(string modId)
        {
            foreach (var item in loadedMods)
            {
                if (item.Info.Id == modId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 加载Mod
        /// </summary>
        /// <param name="modId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ModModuleException"></exception>
        /// <exception cref="ModNotInstalledException"></exception>
        public static ModObject Load(string modId)
        {
            if (string.IsNullOrEmpty(modId))
            {
                throw new ArgumentNullException(nameof(modId));
            }
            var info = installedMods.GetInfo(modId);
            if (info == null)
            {
                throw new ModNotInstalledException(modId, "mod not installed");
            }
            if (IsLoadedMod(modId))
            {
                throw new ModModuleException(info, "mod is loaded");
            }
            var path = installedMods.GetPath(info);
            var modObj = ModLoader.LoadMod(info, path);
            loadedMods.Add(modObj);
            return modObj;
        }

        /// <summary>
        /// 以播放集加载Mod
        /// </summary>
        /// <param name="playlist"></param>
        public static void LoadPlaylist(Playlist playlist)
        {
            Debug.Log("[ModManager] load playlist: " + playlist.Name);
            foreach (var item in playlist.Mods)
            {
                Load(item);
            }
        }

        public static void LoadAllInstalled()
        {
            Debug.Log("[ModManager] load all installed mods.");
            foreach (var item in InstalledMods)
            {
                Load(item.Id);
            }
        }

        public static void EnableAll()
        {
            Debug.Log("[ModManager] enable all");
            foreach (var mod in loadedMods)
            {
                mod.Enable();
            }
        }

        public static void DisableAll()
        {
            Debug.Log("[ModManager] disable all");
            foreach (var mod in loadedMods)
            {
                mod.Disable();
            }
        }
    }
}
