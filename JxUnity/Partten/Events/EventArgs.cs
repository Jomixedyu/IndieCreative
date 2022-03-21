
public class EventArgsBase
{
    public T To<T>() where T : EventArgsBase => this as T;
}

public class CommonEventArgValue<T> : EventArgsBase
{
    public T value;
    public CommonEventArgValue(T value) => this.value = value;
    public CommonEventArgValue<T> SetValue(T value)
    {
        this.value = value;
        return this;
    }
    public override bool Equals(object obj) => value?.Equals(obj) ?? false;
    public override int GetHashCode() => value?.GetHashCode() ?? 0;
    public static implicit operator T(CommonEventArgValue<T> self) => self.value;
    public static implicit operator CommonEventArgValue<T>(T value) => new CommonEventArgValue<T>(value);

}
public class CommonEventArgString : CommonEventArgValue<string>
{
    public CommonEventArgString(string value) : base(value)
    {
    }
    public static CommonEventArgString Create(string value) => new CommonEventArgString(value);
}
public class CommonEventArgInt : CommonEventArgValue<int>
{
    public CommonEventArgInt(int value) : base(value)
    {
    }
    public static CommonEventArgInt Create(int value) => new CommonEventArgInt(value);
}
