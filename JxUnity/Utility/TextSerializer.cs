using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

public class TextSerializerSettings
{
    public bool IsWriteTypename { get; set; } = false;
    public bool IsStringEnum { get; set; } = false;
}

public abstract class TextSerializer
{
    private static TextSerializer _xml;
    private static TextSerializer _ujson;
    private static TextNewtonJsonSerializer _ntJson;

    /// <summary>
    /// System.Xml
    /// </summary>
    public static TextSerializer Xml => _xml ?? (_xml = new TextXmlSerializer());

    /// <summary>
    /// unity json
    /// </summary>
    public static TextSerializer uJson => _ujson ?? (_ujson = new TextJsonSerializer());

    private static bool isTriedGetNtJson = false;

    /// <summary>
    /// newtonsoft.json，如果程序集不存在则为null
    /// </summary>
    public static TextNewtonJsonSerializer ntJson
    {
        get
        {
            if (_ntJson != null)
            {
                return _ntJson;
            }
            if (!isTriedGetNtJson)
            {
                if (TextNewtonJsonSerializer.HasAssembly())
                {
                    _ntJson = new TextNewtonJsonSerializer();
                }
                isTriedGetNtJson = true;
            }
            return _ntJson; //nullable
        }
    }


    /// <summary>
    /// 首选newtonJson，如果不存在则使用unityJson
    /// </summary>
    public static TextSerializer Json
    {
        get
        {
            if (ntJson != null)
            {
                return ntJson;
            }
            return uJson;
        }
    }

    public abstract string Format { get; }
    public abstract string Serialize(object obj, bool isReadability = false, TextSerializerSettings settings = null);
    public abstract object Deserialize(string text, Type type, TextSerializerSettings settings = null);

    public T Deserialize<T>(string text, TextSerializerSettings settings = null)
    {
        return (T)Deserialize(text, typeof(T), settings);
    }

    public object Read(string path, Type type, TextSerializerSettings settings = null)
    {
        return Deserialize(File.ReadAllText(path), type, settings);
    }
    public T Read<T>(string path, TextSerializerSettings settings = null)
    {
        return (T)Read(path, typeof(T), settings);
    }

    public void ReadAsync(string path, Type type, Action<object, Exception> callback, TextSerializerSettings settings = null)
    {
        var syncCtx = SynchronizationContext.Current;
        Task.Run(() =>
        {
            Exception e = null;
            object result = null;
            try
            {
                result = this.Read(path, type, settings);
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
    public Task<T> ReadAsync<T>(string path, TextSerializerSettings settings = null)
    {
        return Task<T>.Run(() =>
        {
            Exception e = null;
            T result = default;
            try
            {
                result = (T)this.Read(path, typeof(T), settings);
            }
            catch (Exception _e)
            {
                e = _e;
            }
            return result;
        });
    }

    public Task<object> ReadAsync(string path, Type type, TextSerializerSettings settings = null)
    {
        return Task<object>.Run(() =>
        {
            Exception e = null;
            object result = null;
            try
            {
                result = this.Read(path, type, settings);
            }
            catch (Exception _e)
            {
                e = _e;
            }
            return result;
        });
    }

    public static void Write(string path, string content)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        File.WriteAllText(path, content);
    }

    public void Write(string path, object obj, bool isReadability = false, TextSerializerSettings settings = null)
    {
        if (obj == null)
        {
            return;
        }
        Write(path, Serialize(obj, isReadability, settings));
    }

    public void WriteAsync(
        string path,
        object obj,
        bool isReadability = false,
        TextSerializerSettings settings = null,
        Action<Exception> callback = null)
    {
        var syncCtx = SynchronizationContext.Current;
        Task.Run(() =>
        {
            Exception e = null;
            try
            {
                Write(path, obj, isReadability, settings);
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

    public Task<Exception> WriteAsync(
        string path,
        object obj,
        bool isReadability = false,
        TextSerializerSettings settings = null)
    {
        return Task<Exception>.Run(() =>
        {
            Exception e = null;
            try
            {
                Write(path, obj, isReadability, settings);
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

    public override string Serialize(object obj, bool isReadability = false, TextSerializerSettings settings = null)
    {
        XmlSerializer xs = new XmlSerializer(obj.GetType());
        StringBuilder sb = new StringBuilder();
        UTF8StringWriter sw = new UTF8StringWriter(sb);
        xs.Serialize(sw, obj);
        sw.Close();
        return sb.ToString();
    }
    public override object Deserialize(string text, Type type, TextSerializerSettings settings = null)
    {
        XmlSerializer xs = new XmlSerializer(type);
        StringReader sr = new StringReader(text);
        return xs.Deserialize(sr);
    }
}

public class TextJsonSerializer : TextSerializer
{
    public override string Format => "json";

    public override object Deserialize(string text, Type type, TextSerializerSettings settings = null)
    {
        return UnityEngine.JsonUtility.FromJson(text, type);
    }

    public override string Serialize(object obj, bool isReadability = false, TextSerializerSettings settings = null)
    {
        return UnityEngine.JsonUtility.ToJson(obj, isReadability);
    }
}

public class TextNewtonJsonSerializer : TextSerializer
{
    public override string Format => "json";

    private MethodInfo serMethod;


    private Type JsonSerializerSettings;

    private MethodInfo deser;

    private Enum Formatting_Indented;
    private Enum Formatting_None;
    private Enum TypeNameHandling_Objects;

    private object StringEnumConverter;

    public static bool HasAssembly()
    {
        return Assembly.Load("Newtonsoft.Json") != null;
    }

    private object GenSerializerSettings(bool typename, bool stringenum)
    {
        if (!typename && !stringenum)
        {
            return null;
        }
        object settings = Activator.CreateInstance(this.JsonSerializerSettings);
        if (typename)
        {
            var prop = this.JsonSerializerSettings.GetProperty("TypeNameHandling");
            prop.SetValue(settings, this.TypeNameHandling_Objects);
        }
        if (stringenum)
        {
            var propInfo = this.JsonSerializerSettings.GetProperty("Converters");
            var prop = propInfo.GetValue(settings);
            var method = propInfo.PropertyType.GetInterface("ICollection`1").GetMethod("Add");
            method.Invoke(prop, new object[] { this.StringEnumConverter });
        }
        return settings;
    }

    public TextNewtonJsonSerializer()
    {
        var ass = Assembly.Load("Newtonsoft.Json");
        var JsonConvert = ass.GetType("Newtonsoft.Json.JsonConvert");
        this.JsonSerializerSettings = ass.GetType("Newtonsoft.Json.JsonSerializerSettings");
        var Formatting = ass.GetType("Newtonsoft.Json.Formatting");

        this.serMethod = JsonConvert.GetMethod("SerializeObject", new Type[] { typeof(object), Formatting, JsonSerializerSettings });

        this.deser = JsonConvert.GetMethod("DeserializeObject", new Type[] { typeof(string), typeof(Type), this.JsonSerializerSettings });

        this.Formatting_None = (Enum)Enum.Parse(Formatting, "None");
        this.Formatting_Indented = (Enum)Enum.Parse(Formatting, "Indented");

        var TypeNameHandling = ass.GetType("Newtonsoft.Json.TypeNameHandling");
        this.TypeNameHandling_Objects = (Enum)Enum.Parse(TypeNameHandling, "Objects");

        Type StringEnumConverter = ass.GetType("Newtonsoft.Json.Converters.StringEnumConverter");
        this.StringEnumConverter = Activator.CreateInstance(StringEnumConverter);
    }

    public override object Deserialize(string text, Type type, TextSerializerSettings settings = null)
    {
        if (this.deser == null)
        {
            throw new NotImplementedException();
        }

        bool isTypename = settings?.IsWriteTypename ?? false;
        bool stringEnum = settings?.IsStringEnum ?? true;

        var s = GenSerializerSettings(isTypename, stringEnum);

        return this.deser.Invoke(null, new object[] { text, type, s });
    }

    public override string Serialize(object obj, bool isReadability, TextSerializerSettings settings)
    {
        if (this.serMethod == null)
        {
            throw new NotImplementedException();
        }
        bool isTypename = settings?.IsWriteTypename ?? false;
        bool stringEnum = settings?.IsStringEnum ?? true;

        var s = GenSerializerSettings(isTypename, stringEnum);

        object[] ctorparam = new object[] { obj, isReadability ? this.Formatting_Indented : this.Formatting_None, s };

        return (string)this.serMethod.Invoke(null, ctorparam);
    }
}
