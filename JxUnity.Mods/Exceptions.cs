using System;

namespace JxUnity.Mods
{
    public class ModException : Exception
    {
        public ModException(string msg) : base(msg)
        {

        }
    }

    public class ModModuleException : ModException
    {
        private ModInfo modInfo;

        public ModInfo ModInfo => modInfo;
        public ModModuleException(ModInfo modInfo, string msg) : base(msg)
        {
            this.modInfo = modInfo;
        }
        public override string ToString()
        {
            return $"modInfo: {modInfo}, {Message}";
        }
    }

    public class ModNotInstalledException : ModException
    {
        private string modId;

        public ModNotInstalledException(string modId, string msg) : base(msg)
        {
            this.modId = modId;
        }
        public override string ToString()
        {
            return $"modId: {modId}, {Message}";
        }
    }


}
