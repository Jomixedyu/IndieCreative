using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JxUnity.SaveDatas
{
    public class GameSave
    {
        public static SaveData Current { get; }
        public static string SaveFolderName = "SaveData";

        public string Time { get; }
        public string CaptureName { get; }
        public string Note { get; }

    }
}
