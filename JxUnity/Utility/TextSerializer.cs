using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

public abstract class TextSerializer
{
    /// <summary>
    /// System.Xml
    /// </summary>
    public static TextSerializer Xml { get; } = new TextXmlSerializer();
    /// <summary>
    /// Newtonsoft.Json
    /// </summary>
    public static TextSerializer Json { get; } = new TextNewtonJsonSerializer();

    public abstract string Format { get; }
    public abstract string Serialize(object obj);
    public abstract object Deserialize(string text, Type type);

    public object Read(string path, Type type)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        return Deserialize(File.ReadAllText(path), type);
    }
    public T Read<T>(string path)
    {
        return (T)Read(path, typeof(T));
    }

    public void Write(string path, object obj)
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

    public Task<Exception> WriteAsync(string path, object obj)
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
    public void WriteAsync(string path, object obj, Action<Exception> result)
    {
        Task.Run(() =>
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
            result?.Invoke(e);
        });
    }

    public class UTF8StringWriter : StringWriter
    {
        private static readonly Encoding UTF8 = new UTF8Encoding(false);
        public override Encoding Encoding => UTF8;
        public UTF8StringWriter(StringBuilder sb) : base(sb) { }
    }
}

public class TextXmlSerializer : TextSerializer
{
    public override string Format => "xml";

    public override string Serialize(object obj)
    {
        XmlSerializer xs = new XmlSerializer(obj.GetType());
        StringBuilder sb = new StringBuilder();
        UTF8StringWriter sw = new UTF8StringWriter(sb);
        xs.Serialize(sw, obj);
        sw.Close();
        return sb.ToString();
    }
    public override object Deserialize(string text, Type type)
    {
        XmlSerializer xs = new XmlSerializer(type);
        StringReader sr = new StringReader(text);
        return xs.Deserialize(sr);
    }
}
public class TextNewtonJsonSerializer : TextSerializer
{
    public override string Format => "json";

    private Func<object, string> ser;
    private Func<string, Type, object> deser;

    public TextNewtonJsonSerializer()
    {
        var ass = Assembly.Load("Newtonsoft.Json");
        var type = ass.GetType("Newtonsoft.Json.JsonConvert");

        var serMethod = type.GetMethod("SerializeObject", new Type[] { typeof(object) });
        this.ser = (Func<object, string>)serMethod.CreateDelegate(typeof(Func<object, string>));

        var deserMethod = type.GetMethod("DeserializeObject", new Type[] { typeof(string), typeof(Type) });
        this.deser = (Func<string, Type, object>)deserMethod.CreateDelegate(typeof(Func<string, Type, object>));
    }

    public override object Deserialize(string text, Type type)
    {
        if (this.deser == null)
        {
            throw new NotImplementedException();
        }
        return this.deser.Invoke(text, type);
    }

    public override string Serialize(object obj)
    {
        if (this.ser == null)
        {
            throw new NotImplementedException();
        }
        return this.ser.Invoke(obj);
    }
}
