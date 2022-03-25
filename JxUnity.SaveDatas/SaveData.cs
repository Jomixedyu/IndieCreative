using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JxUnity.SaveDatas
{
    //UTF-8 with BOM
    public class SaveData
    {
        private Dictionary<string, Value> datas
            = new Dictionary<string, Value>();

        private void Set<T>(string path, T value)
        {
            if (this.datas.TryGetValue(path, out var v))
            {
                ValuesPool.Push(v);
                this.datas[path] = ValuesPool.Pop<Value<T>>().SetValue(value);
            }
            else
            {
                this.datas.Add(path, ValuesPool.Pop<Value<T>>().SetValue(value));
            }
        }

        private T Get<T>(string path)
        {
            if (this.datas.TryGetValue(path, out var v))
            {
                Value<T> value = v as Value<T>;
                if (value != null)
                {
                    return value.value;
                }
            }
            throw new ArgumentException("not exist: " + path);
        }

        public bool HasValue(string path) => this.datas.ContainsKey(path);

        public int GetInt(string path) => Get<int>(path);
        public void SetInt(string path, int value) => Set(path, value);

        public float GetFloat(string path) => Get<float>(path);
        public void SetFloat(string path, float value) => Set(path, value);

        public string GetString(string path) => Get<string>(path);
        public void SetString(string path, string value) => Set(path, value);

        public bool GetBool(string path) => Get<bool>(path);
        public void SetBool(string path, bool value) => Set(path, value);

        public void SetObject(string path, object value)
        {
            foreach (var item in SaveDataUtility.Unpack(path, value))
            {
                if (this.datas.TryGetValue(item.name, out var oldv))
                {
                    ValuesPool.Push(oldv);
                    this.datas[item.name] = ValuesPool.PopOrCreate(item.type, item.value);
                }
                else
                {
                    this.datas.Add(item.name, ValuesPool.PopOrCreate(item.type, item.value));
                }
            }
        }

        public object GetObject(string path, Type type)
        {
            return SaveDataUtility.Pack(path, type, s =>
            {
                if (this.datas.TryGetValue(s, out var oldv))
                {
                    return oldv;
                }
                return null;
            });
        }

        public static SaveData Load(Stream stream)
        {
            SaveData saveData = new SaveData();
            BinaryReader br = new BinaryReader(stream);

            (string name, string type, string value) info = default;

            while ((info = SaveDataUtility.DeserializeRecord(br)).name != null)
            {
                var v = ValuesPool.PopOrCreate(info.type, info.value);
                if (v != null)
                {
                    saveData.datas.Add(info.name, v);
                }
            }

            return saveData;
        }

        private static StringBuilder sb = new StringBuilder();

        public string SerializeText()
        {
            sb.Clear();
            foreach (var item in this.datas)
            {
                var rec = SaveDataUtility.SerializeRecord(item.Key, item.Value.TypeName, item.Value.ToString());
                sb.Append(rec);
                sb.Append('\n');
            }
            var str = sb.ToString();
            sb.Clear();
            return str;
        }

    }
}
