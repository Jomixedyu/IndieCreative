using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

public class TextSerializer
{
    public interface ITextSerializer
    {
        string Format { get; }
        string Serialize(object obj);
        object Deserialize(string text, Type type);
    }
    public class TextXmlSerializer : ITextSerializer
    {
        public class UTF8StringWriter : StringWriter
        {
            private static readonly Encoding UTF8 = new UTF8Encoding(false);
            public override Encoding Encoding => UTF8;
            public UTF8StringWriter(StringBuilder sb) : base(sb) { }
        }

        public string Format => "xml";

        public string Serialize(object obj)
        {
            XmlSerializer xs = new XmlSerializer(obj.GetType());
            StringBuilder sb = new StringBuilder();
            UTF8StringWriter sw = new UTF8StringWriter(sb);
            xs.Serialize(sw, obj);
            sw.Close();
            return sb.ToString();
        }
        public object Deserialize(string text, Type type)
        {
            XmlSerializer xs = new XmlSerializer(type);
            StringReader sr = new StringReader(text);
            return xs.Deserialize(sr);
        }
    }

    public static ITextSerializer Serializer { get; set; } = new TextXmlSerializer();

    public static string Serialize(object obj)
    {
        return Serializer.Serialize(obj);
    }
    public static object Deserialize(string text, Type type)
    {
        return Serializer.Deserialize(text, type);
    }

    public static T Deserialize<T>(string text)
    {
        return (T)Deserialize(text, typeof(T));
    }

    public static object Read(string path, Type type)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        return Deserialize(File.ReadAllText(path), type);
    }
    public static T Read<T>(string path)
    {
        return (T)Read(path, typeof(T));
    }
    public static void Write(string path, object obj)
    {
        if (obj == null)
        {
            return;
        }
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        File.WriteAllText(path, Serialize(obj));
    }

    public static Task<Exception> WriteAsync(string path, object obj)
    {
        return Task.Run<Exception>(() =>
        {
            Exception e = null;
            try
            {
                Write(path, obj);
            }
            catch (Exception _e)
            {
                e = _e;
            }
            return e;
        });
    }
    public static void WriteAsync(string path, object obj, Action<Exception> result)
    {
        Task.Run(() => {
            Exception e = null;
            try
            {
                Write(path, obj);
            }
            catch (Exception _e)
            {
                e = _e;
            }
            result?.Invoke(e);
        });
    }

}
