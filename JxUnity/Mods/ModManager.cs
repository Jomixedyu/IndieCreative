using JxUnity.Mods.UserInterface;
using JxUnity.Mods.UserInterfaceImpl;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JxUnity.Mods
{
    public class ModManager
    {
        private static List<ModObject> mods;

        public static ModEventHub ModEventHub;

        public static ISystem System { get; private set; } = new SystemImpl();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Init()
        {
            mods = new List<ModObject>();
            ModEventHub = new ModEventHub();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init2()
        {
            Application.quitting += Application_quitting;
        }

        private static void Application_quitting()
        {
            DisableAll();
        }


        public static void Load(string path)
        {
            mods.Add(ModLoader.LoadMod(path));
        }

        public static void BatchLoadFromFolder(string folder)
        {
            var dirs = Directory.GetDirectories(folder);
            foreach (var dir in dirs)
            {
                Load(dir);
            }
        }

        public static void EnableAll()
        {
            Debug.Log("[ModManager] enable all");
            foreach (var mod in mods)
            {
                mod.Enable();
            }
        }
        public static void DisableAll()
        {
            Debug.Log("[ModManager] disable all");
            foreach (var mod in mods)
            {
                mod.Disable();
            }
        }
    }
}
