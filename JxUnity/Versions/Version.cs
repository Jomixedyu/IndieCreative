using System;
using System.Collections.Generic;

namespace JxUnity.Versions
{
    public class Version
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }

        public Version()
        {

        }

        public Version(string version)
        {
            var arr = version.Split('.');
            this.Major = int.Parse(arr[0]);
            this.Minor = int.Parse(arr[1]);
            this.Build = int.Parse(arr[2]);
        }

        public Version(int major, int minor, int build)
        {
            this.Major = major;
            this.Minor = minor;
            this.Build = build;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}";
        }

        public override bool Equals(object obj)
        {
            Version v = obj as Version;
            if (object.ReferenceEquals(v, null))
            {
                return false;
            }
            return this.Major == v.Major && this.Minor == v.Minor && this.Build == v.Build;
        }

        public override int GetHashCode()
        {
            int hashCode = -1536346269;
            hashCode = hashCode * -1521134295 + this.Major.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Minor.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Build.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Version x, Version y) => x.Equals(y);
        public static bool operator !=(Version x, Version y) => !x.Equals(y);
        public static bool operator >(Version x, Version y)
        {
            if (x.Major > y.Major) return true;
            if (x.Minor > y.Minor) return true;
            if (x.Build > y.Build) return true;
            return false;
        }
        public static bool operator <(Version x, Version y)
        {
            if (x.Major < y.Major) return true;
            if (x.Minor < y.Minor) return true;
            if (x.Build < y.Build) return true;
            return false;
        }
        public static bool operator >=(Version x, Version y)
        {
            return x > y || x == y;
        }
        public static bool operator <=(Version x, Version y)
        {
            return x < y || x == y;
        }
    }
}
