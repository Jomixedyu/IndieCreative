using System;
using System.IO;
using System.Reflection;


namespace JxUnity.Mods
{
    public class ModException : Exception
    {
        public ModException(string msg) : base(msg)
        {
        }
    }

    internal static class ModLoader
    {
        public static ModObject LoadMod(string path)
        {
            string cfg = path + "/config.xml";

            ModInfo modInfo;
            using (var fs = File.OpenRead(cfg))
            {
                modInfo = Serializer.DeserializeFromFile(fs);
            }

            Action funEnable = null, funDisable = null;
            if (!string.IsNullOrWhiteSpace(modInfo.Script?.ScriptName))
            {
                var scriptPath = path + "/" + modInfo.Script.ScriptName;
                Assembly ass = Assembly.LoadFrom(scriptPath);
                var type = ass.GetType(modInfo.Script.Class, false, false);
                if (type == null)
                {
                    throw new ModException($"class not found. mod: {modInfo.Id}");
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
