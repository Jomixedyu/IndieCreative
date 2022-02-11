using System;
using System.Collections.Generic;
using System.IO;

namespace JxUnity.SaveDatas
{
    public static class OptionsData
    {
        public static SaveData Data { get; private set; }

        public static readonly string WorkingFolderName = "Config";
        public static readonly string Filename = "Options.cfg";
        public static readonly string FullFilename = WorkingFolderName + "/" + Filename;
        public static readonly string WorkingFolderPath = PathHelper.GetWritablePath() + "/" + WorkingFolderName;

        public static void Reload()
        {
            if (!File.Exists(FullFilename))
            {
                Data = new SaveData();
                return;
            }
            var ser = File.ReadAllText(FullFilename);
            Data = SaveData.Load(ser);
        }

        public static void Save()
        {
            if (Data == null)
            {
                return;
            }
            if (!Directory.Exists(WorkingFolderPath))
            {
                Directory.CreateDirectory(WorkingFolderPath);
            }
            File.WriteAllText(FullFilename, Data.SerializeText());
        }

    }
}
