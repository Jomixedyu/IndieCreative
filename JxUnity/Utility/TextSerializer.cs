using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

public abstract class TextSerializer
{
    private static TextSerializer _xml;
    private static TextSerializer _json;

    /// <summary>
    /// System.Xml
    /// </summary>
    public static TextSerializer Xml => _xml ?? (_xml = new TextXmlSerializer());

    /// <summary>
    /// UnityJson
    /// </summary>
    public static TextSerializer Json => _json ?? (_json = new TextJsonSerializer());

    public abstract string Format { get; }
    public abstract string Serialize(object obj, bool isReadability = false);
    public abstract object Deserialize(string text, Type type);

    public T Deserialize<T>(string text)
    {
        return (T)Deserialize(text, typeof(T));
    }

    public object Read(string path, Type type)
    {
        return Deserialize(File.ReadAllText(path), type);
    }
    public T Read<T>(string path)
    {
        return (T)Read(path, typeof(T));
    }

    public void ReadAsync(string path, Type type, Action<object, Exception> callback)
    {
        var syncCtx = SynchronizationContext.Current;
        Task.Run(() =>
        {
            Exception e = null;
            object result = null;
            try
            {
                result = this.Read(path, type);
            }
            catch (Exception _e)
            {
                e = _e;
            }
            finally
            {
                syncCtx.Send(_ => callback?.Invoke(result, e), null);
            }
        });
    }
    public Task<T> ReadAsync<T>(string path)
    {
        return Task<T>.Run(() =>
        {
            Exception e = null;
            T result = default;
            try
            {
                result = (T)this.Read(path, typeof(T));
            }
            catch (Exception _e)
            {
                e = _e;
            }
            return result;
        });
    }

    public Task<object> ReadAsync(string path, Type type)
    {
        return Task<object>.Run(() =>
        {
            Exception e = null;
            object result = null;
            try
            {
                result = this.Read(path, type);
            }
            catch (Exception _e)
            {
                e = _e;
            }
            return result;
        });
    }

    public void Write(string path, object obj, bool isReadability = false)
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
        File.WriteAllText(path, Serialize(obj, isReadability));
    }

    public void WriteAsync(string path, object obj, bool isReadability = false, Action<Exception> callback = null)
    {
        var syncCtx = SynchronizationContext.Current;
        Task.Run(() =>
        {
            Exception e = null;
            try
            {
                Write(path, obj, isReadability);
            }
            catch (Exception _e)
            {
                e = _e;
            }
            finally
            {
                syncCtx.Send(_e => callback?.Invoke(_e as Exception), e);
            }
        });
    }

    public Task<Exception> WriteAsync(string path, object obj, bool isReadability = false)
    {
        return Task<Exception>.Run(() =>
        {
            Exception e = null;
            try
            {
                Write(path, obj, isReadability);
            }
            catch (Exception _e)
            {
                e = _e;
            }
            return e;
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

    public override string Serialize(object obj, bool isReadability = false)
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

public class TextJsonSerializer : TextSerializer
{
    public override string Format => "json";

    public override object Deserialize(string text, Type type)
    {
        return UnityEngine.JsonUtility.FromJson(text, type);
    }

    public override string Serialize(object obj, bool isReadability = false)
    {
        return UnityEngine.JsonUtility.ToJson(obj, isReadability);
    }
}

public class TextNewtonJsonSerializer : TextSerializer
{
    public override string Format => "json";

    private MethodInfo serMethod;
    private Func<string, Type, object> deser;

    private Enum indented;
    private Enum none;

    public TextNewtonJsonSerializer()
    {
        var ass = Assembly.Load("Newtonsoft.Json");
        var type = ass.GetType("Newtonsoft.Json.JsonConvert");

        var formatting = ass.GetType("Newtonsoft.Json.Formatting");

        this.serMethod = type.GetMethod("SerializeObject", new Type[] { typeof(object), formatting });

        var deserMethod = type.GetMethod("DeserializeObject", new Type[] { typeof(string), typeof(Type) });
        this.deser = (Func<string, Type, object>)deserMethod.CreateDelegate(typeof(Func<string, Type, object>));

        this.none = (Enum)Enum.Parse(formatting, "None");
        this.indented = (Enum)Enum.Parse(formatting, "Indented");
    }

    public override object Deserialize(string text, Type type)
    {
        if (this.deser == null)
        {
            throw new NotImplementedException();
        }
        return this.deser.Invoke(text, type);
    }

    public override string Serialize(object obj, bool isReadability = false)
    {
        if (this.serMethod == null)
        {
            throw new NotImplementedException();
        }
        object[] ctorparam = new object[] { obj, isReadability ? this.indented : this.none };
        return (string)this.serMethod.Invoke(null, ctorparam);
    }
}
