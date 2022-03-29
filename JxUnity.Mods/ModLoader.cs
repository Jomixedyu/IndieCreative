using System;
using System.IO;
using System.Reflection;


namespace JxUnity.Mods
{

    internal static class ModLoader
    {
        public static ModInfo LoadInfo(string cfgPath)
        {
            ModInfo modInfo;
            using (var fs = File.OpenRead(cfgPath))
            {
                modInfo = Serializer.DeserializeModInfoFromFile(fs);
            }
            return modInfo;
        }

        public static ModObject LoadMod(ModInfo modInfo, string modPath)
        {
            Action funEnable = null, funDisable = null;
            if (!string.IsNullOrWhiteSpace(modInfo.Script?.ScriptName))
            {
                var scriptPath = modPath + "/" + modInfo.Script.ScriptName;
                Assembly ass = Assembly.LoadFrom(scriptPath);
                var type = ass.GetType(modInfo.Script.Class, false, false);
                if (type == null)
                {
                    throw new ModModuleException(modInfo, $"class not found.");
                }
                var binding = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
                funEnable = type.GetMethod("OnEnable", binding)?.CreateDelegate(typeof(Action)) as Action;
                funDisable = type.GetMethod("OnDisable", binding)?.CreateDelegate(typeof(Action)) as Action;

            }

            ModObject mod = new ModObject(modInfo, funEnable, funDisable);

            return mod;
        }

    }
}
