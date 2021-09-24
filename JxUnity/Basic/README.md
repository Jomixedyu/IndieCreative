# JxUnity.Basic

基础模板

## 单例模板

继承`Singleton`

```C#
public class Test : Singleton<Test>
{
}
```

使用`Test.Instance`或`Test.GetInstance()`获取实例。

使用`Test.HasIntance`判断是否存在实例

## Mono单例模板

继承`MonoSingleton`

```C#
public class Test : MonoSingleton<Test>
{
    protected override void Awake()
    {
        if(!CheckInstanceAndDestroy()) return;
        SetInstance(this);
    }
}
```

使用`Test.Instance`或`Test.GetInstance()`获取实例。

使用`Test.HasIntance`判断是否存在实例

如果单例在新的场景中遇到了同样的实例时，则会销毁旧的实例，并设置新场景的实例。

## 状态机模板

