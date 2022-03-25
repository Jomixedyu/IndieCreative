using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JxUnity.Platforms
{
    public interface IPlatform
    {

        string GetFolderPath(string type);
    }

    public class LocalPlatform : IPlatform
    {
        public string GetFolderPath(string type)
        {
            throw new NotImplementedException();
        }
    }
    public class SteamPlatform : IPlatform
    {
        public string GetFolderPath(string type)
        {
            throw new NotImplementedException();
        }

    }
}
