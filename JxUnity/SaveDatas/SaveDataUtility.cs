using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JxUnity.SaveDatas
{
    internal class SaveDataUtility
    {
        private static Type[] saveableTypes = new Type[]
        {
            typeof(string), typeof(bool), typeof(int), typeof(float)
        };
        public static bool IsSaveableType(Type type)
        {
            return saveableTypes.Contains(type);
        }
        public static string SerializeRecord(string name, string type, string value)
        {
            return $"{name}:{type}={value}";
        }
        public static (string name, string type, string value) DeserializeRecord(string record)
        {
            var s1 = record.IndexOf(':');
            string name = record.Substring(0, s1);
            var s2 = record.IndexOf("=");
            string type = record.Substring(s1 + 1, s2 - s1 - 1);
            string value = record.Substring(s2 + 1);
            return (name, type, value);
        }

        private static void Unpack(FieldInfo info, object inst, string prefix, IList<(string name, string type, string value)> output)
        {
            var fieldName = info.Name;
            var fieldType = info.FieldType;
            var fieldValue = info.GetValue(inst);

            string _prefix = string.IsNullOrWhiteSpace(prefix) ? string.Empty : prefix + ".";
            if (IsSaveableType(fieldType))
            {
                output.Add(($"{_prefix}{fieldName}", fieldType.Name, fieldValue.ToString()));
            }
            else
            {
                foreach (var field in fieldType.GetFields())
                {
                    Unpack(field, fieldValue, $"{_prefix}{fieldName}", output);
                }
            }
        }
        public static IList<(string name, string type, string value)> Unpack(string path, object obj)
        {
            var type = obj.GetType();
            var lst = new List<(string name, string type, string value)>();

            string _prefix = string.IsNullOrWhiteSpace(path) ? string.Empty : path + ".";
            _prefix += type.Name;
            foreach (var field in type.GetFields())
            {
                Unpack(field, obj, _prefix, lst);
            }
            return lst;
        }

        private static void Pack(FieldInfo info, object inst, string path, Func<string, Value> dataProvider)
        {
            var fieldName = info.Name;
            var fieldType = info.FieldType;

            path = (string.IsNullOrEmpty(path) ? "" : path + ".") + info.Name;
            if (IsSaveableType(fieldType))
            {
                var rst = dataProvider(path);
                if (rst != null)
                {
                    info.SetValue(inst, rst.ObjValue);
                }
            }
            else
            {
                var fieldInst = Activator.CreateInstance(fieldType);
                info.SetValue(inst, fieldInst);

                foreach (var field in fieldType.GetFields())
                {
                    Pack(field, fieldInst, $"{path}{fieldName}", dataProvider);
                }
            }

        }

        public static object Pack(string path, Type type, object _inst, Func<string, Value> dataProvider)
        {
            var inst = _inst ?? Activator.CreateInstance(type);

            path = (string.IsNullOrEmpty(path) ? "" : path + ".") + type.Name;
            foreach (var info in type.GetFields())
            {
                Pack(info, inst, path, dataProvider);
            }

            return inst;
        }
    }
}
