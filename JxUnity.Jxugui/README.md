
# JxUnity.Jxugui
Unity UGUI面板编写，轻松编写UI。  
当前版本：v2.2.0

## Content
- [JxUnity.Jxugui](#jxunityjxugui)
  - [Content](#content)
  - [UI管理器](#ui管理器)
  - [顶级UI面板声明](#顶级ui面板声明)
  - [JuiPanel特性](#juipanel特性)
    - [`Name` UI名字，如果和对象名或类型名相同则可省略](#name-ui名字如果和对象名或类型名相同则可省略)
    - [`EnableUpdate` 启用Update函数](#enableupdate-启用update函数)
    - [`ResourcePath` 资源加载路径，当UI不存在于场景时，进行动态加载](#resourcepath-资源加载路径当ui不存在于场景时进行动态加载)
    - [`IsAutoBindElement` 自动绑定元素，对类型内标示的字段进行自动绑定](#isautobindelement-自动绑定元素对类型内标示的字段进行自动绑定)
    - [`IsPreBind` 在场景初始化时将脚本绑定到UI](#isprebind-在场景初始化时将脚本绑定到ui)
    - [Example](#example)
  - [子元素绑定](#子元素绑定)
  - [子UI面板](#子ui面板)
  - [消息系统](#消息系统)
  - [UI调试用监视面板](#ui调试用监视面板)
  - [帮助](#帮助)

## UI管理器
`JuiManager`可以是一个单例类，可以使用`JuiManager.Instance`来访问实例。  
`JuiManager`应当挂在ui画布上，每个一级的子对象都应是一个UI面板，管理器仅用来管理顶级UI。  
`JuiManager`是一个`DontDestroyOnLoad`的对象，在所有场景内都存在，如果多个场景内存在`JuiManager`，则加载新场景时，新场景的所有顶级UI会归属到全局管理器下，在移交管理权后，该场景管理器自动销毁。

## 顶级UI面板声明
顶级UI：在Canvas下的一级UI，直接由管理器来管理，每一个顶级UI都是一个**单例**。  
如果想去编写一个UI代码，则需要继承`JuiBase`类型，如：
```c#
public class UITest : JuiBase<UITest>
{

}
```
如果想让这个面板生效，还需要给类增加一个特性：
```c#
[JuiPanel]
public class UITest : JuiBase<UITest>
{
    
}
```
现在就可以轻松地访问这个UI面板了：
```c#
UITest.Instance.Show();
UITest.Instance.Hide();
```

UI管理器中维护着一个UI栈，你可以在UI面板中访问到当前是否为焦点的信息。
```c#
[JuiPanel(EnableUpdate = true)]
public class UITest : JuiBase<UITest>
{
    protected override void OnUpdate()
    {
        if(IsFocus)
        {
            if(Input.GetKey(...)) {...}
        }
    }
}

```

## JuiPanel特性

### `Name` UI名字，如果和对象名或类型名相同则可省略
如果没有没有使用`Name`属性，则默认使用类名作为UI的名字，在该样例中并没有声明`Name`属性，所以UI的名字为`UITest`，绑定时会以这么名字查找。

### `EnableUpdate` 启用Update函数
该属性默认为false，如设置为true，将会启用该类的OnUpdate函数。

### `ResourcePath` 资源加载路径，当UI不存在于场景时，进行动态加载
当UI不存在与场景时，加载并将物体实例化至场景中后，在进行绑定。如果想实现某个面板的自定义加载过程，请重写`LoadResource`方法。

### `IsAutoBindElement` 自动绑定元素，对类型内标示的字段进行自动绑定
该属性默认为true，即在UI面板实例化期间，会自动将场景内的UI对象绑定至面板中。

### `IsPreBind` 在场景初始化时将脚本绑定到UI
该属性默认为false  
如设置为true，将会在管理器初始化阶段就将该UI面板实例化  
如设置为false，UI面板将会推迟至第一次调用时实例化。  

### Example
一个启用预绑定和OnUpdate函数的UI
```c#
[JuiPanel(IsPreBind = true, EnableUpdate = true)]
public class UITest : JuiBase<UITest>
{
    protected override void OnUpdate()
    {

    }
}
```

## 子元素绑定
可绑定内容：
- GameObject
- Transform
- Component

元素自动绑定使用两种特性：
- JuiElement
- JuiElementArray

因为UGUI都是基于Component的，所以可以直接绑定UI对象。

样例：
```c#
[JuiPanel(IsPreBind = true, EnableUpdate = true)]
public class UITest : JuiBase<UITest>
{
    [JuiElement] Transform child { get; } //property
    [JuiElement] GameObject childGo; //field
    [JuiElement("Buttons/Ok")] Button okBtn;
}
```
如果在`JuiElement`没有传入路径，则使用对象名作为子对象名进行绑定。  
在UiTest这个UI下的名为child的游戏物体上的Transform将会绑定到child属性。  

绑定支持属性和字段。

还可以绑定一个`List<>`或`Array`：
```c#
[JuiPanel(IsPreBind = true, EnableUpdate = true)]
public class UITest : JuiBase<UITest>
{
    [JuiElementArray(typeof(Button))] 
    Button[] btns { get; }
}
```
btns属性中包含了UITest子一级所有的Button对象。


## 子UI面板
子UI面板是为了解决一些顶级UI过于复杂而提出的解决方案，他由父UI管理，这个父UI可以是顶级UI也可以是其他子UI，但并不建议嵌套多层子UI。  

和顶级不一样，子UI**不是单例**。  
所以子UI并不在类型上面声明，只能存在于父UI的实例之中。

使用特性`JuiElementSubPanel`可以使用自动绑定来获取实例  
如果不声明路径将会默认以对象名`Sub`寻找并绑定UI  
样例：
```c#
[JuiPanel(IsPreBind = true, EnableUpdate = true)]
public class UITest : JuiBase<UITest>
{
    [JuiElementSubPanel(EnableUpdate = true)] 
    UISub Sub = default;
}

public class UISub : JuiSubBase
{
    protected override void OnInitialize()
    {
        Debug.Log(this.Parent.Name); // UITest
    }
}
```

如果你想在运行过程中生成子UI，可以使用函数`CreateSubUI`，来动态创建一个该面板的子UI。



## 消息系统

```c#
/// <summary>
/// 消息：初始化
/// </summary>
protected virtual void OnInitialize() { }
/// <summary>
/// 消息：当UI显示时
/// </summary>
protected virtual void OnShow() { }
/// <summary>
/// 消息：当UI隐藏时
/// </summary>
protected virtual void OnHide() { }
/// <summary>
/// 消息：当渲染更新时
/// </summary>
protected virtual void OnUpdate() { }
/// <summary>
/// 消息：当UI销毁时
/// </summary>
protected virtual void OnDestroy() { }
/// <summary>
/// 消息：获取焦点
/// </summary>
protected virtual void OnFocus() { }
/// <summary>
/// 消息：失去焦点
/// </summary>
protected virtual void OnLostFocus() { }
/// <summary>
/// 消息：当有子UI被添加时
/// </summary>
/// <param name="sub"></param>
protected virtual void OnAddSubUI(JuiAbstract sub) { }
/// <summary>
/// 消息：当有子UI被移除时
/// </summary>
/// <param name="sub"></param>
protected virtual void OnRemoveSubUI(JuiAbstract sub) { }
/// <summary>
/// 消息：作为子UI获取焦点
/// </summary>
protected virtual void OnFocusSelf() { }
/// <summary>
/// 消息：作为子UI失去焦点
/// </summary>
protected virtual void OnLostFocusSelf() { }
```

## UI调试用监视面板
本库提供了一个在编辑器运行时查看UI情况的面板，在菜单`Window`>`UI`>`JuiInspector`。

## 帮助
Q：UI在游戏启动时是存在并激活的，但是脚本并没有被执行。  
A：可能是还没有实例化，可以设置`JuiPanel`中的`IsPreBind`属性为true进行初始实例化。