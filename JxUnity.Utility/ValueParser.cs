using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JxUnity.Utility
{
    public static class ValueParser
    {
        public static string ToStructuredData(this Color color)
        {
            return $"{{{color.r},{color.g},{color.b},{color.a}}}";
        }

        public static string ToStructuredData(this IList<Color> color)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            for (int i = 0; i < color.Count; i++)
            {
                sb.Append(color[i].ToStructuredData());
                if (i < color.Count - 1)
                {
                    sb.Append(',');
                }
            }
            sb.Append(']');
            return sb.ToString();
        }

        public static Color Color(string value)
        {
            var f = DeconstructFloats(value);
            var r = f.Length >= 1 ? f[0] : 0;
            var g = f.Length >= 2 ? f[1] : 0;
            var b = f.Length >= 3 ? f[2] : 0;
            var a = f.Length >= 4 ? f[3] : 0;
            return new Color(r, g, b, a);
        }

        public static Vector4 Vec4(string value)
        {
            var f = DeconstructFloats(value);
            var x = f.Length >= 1 ? f[0] : 0;
            var y = f.Length >= 2 ? f[1] : 0;
            var z = f.Length >= 3 ? f[2] : 0;
            var w = f.Length >= 4 ? f[3] : 0;
            return new Vector4(x, y, z, w);
        }

        public static Vector3 Vec3(string value)
        {
            var f = DeconstructFloats(value);
            var x = f.Length >= 1 ? f[0] : 0;
            var y = f.Length >= 2 ? f[1] : 0;
            var z = f.Length >= 3 ? f[2] : 0;
            return new Vector3(x, y, z);
        }

        public static Vector2 Vec2(string value)
        {
            var f = DeconstructFloats(value);
            var x = f.Length >= 1 ? f[0] : 0;
            var y = f.Length >= 2 ? f[1] : 0;
            return new Vector2(x, y);
        }

        private static float[] DeconstructFloats(string value)
        {
            var strs = value.Split(',');
            return Array.ConvertAll(strs, s => Convert.ToSingle(s));
        }

        //public static T[] _ValueArray<T>(string value, ref int pos)
        //{
        //    T[] array = new T[value.Length];
        //    while (pos < value.Length)
        //    {
        //        if (value[pos] == '[')
        //        {
        //            pos++;
        //            _ValueArray<T>(value, ref pos);
        //        }

        //    }
        //}

        //public static T[] ValueArray<T>(string value, ref int pos)
        //{
        //    int pos = 0;
        //    while (pos < value.Length)
        //    {
        //        if (value[pos] == '[')
        //        {
        //            ValueArray<T>(value);
        //        }

        //    }
        //}
    }
}
