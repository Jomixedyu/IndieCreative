# JxUnity.Framework
 ![](https://img.shields.io/github/license/JomiXedYu/JxCode.CoreLib?style=for-the-badge)

面向小型独立游戏的Unity框架，为什么是小型呢？这里没有依赖注入，没有MVC，只有功能好用的好写的模块。  
在JxUnity文件夹下，每一个文件夹都是一个模块，模块与模块之间实现了完全无耦合，可以任意的加载与卸载。

## Content
- [JxUnity.Framework](#jxunityframework)
  - [Content](#content)
  - [Basic](#basic)
  - [AssetManagment](#assetmanagment)
  - [JxUGUI](#jxugui)
  - [Localization](#localization)
  - [Media](#media)
  - [Pool](#pool)
  - [Procedure](#procedure)
  - [Utility](#utility)
  - [xLua](#xlua)

## Basic
该库已进入长期稳定版，[查看文档](JxUnity/Basic/README.md)

基础的单例与Mono单例，状态机等常用设计模板类。
## AssetManagment
资源管理框架完成后独立出去成为单独的库，并改名为ResDB，详见https://github.com/JomiXedYu/JxUnity.ResDB

## JxUGUI
该库已进入长期稳定版，[查看文档](JxUnity/Jxugui/README.md)

通过C#特性来自动绑定元素，脱离了MonoBehaviour，可以轻松在其他框架下如ILRuntime等环境中运行，每个顶级UI都是一个单例，增加了显示栈，焦点消息等功能。
## Localization
类库设计阶段，经常出现不兼容性更新。
## Media
该库已进入长期稳定版，[查看文档](JxUnity/Media/README.md)

实现了音效池与播放器等组件，解决音频截断，以及实现音乐淡入淡出等功能。
## Pool
该库已进入长期稳定版，[查看文档](JxUnity/Pool/README.md)

为C#与Mono对象提供对象池。
## Procedure
该库已进入长期稳定版，[查看文档](JxUnity/Procedure/README.md)

为游戏提供一个易用的全局状态机，只须继承，状态自动绑定。

## Utility
该库将会不断的更新与迭代，但不会出现不兼容的更新，[查看文档](JxUnity/Utility/README.md)

一个类就是一个工具，互相之间没有依赖，可以随意添加与删除。

- 摄像机截屏：使用自定义的摄像机进行叠加截图
- 协程任务：可以在外部暂停，继续，停止的封装。
- 光标工具
- 几何检测：扇形检测，矩形检测等函数。
- 状态信息：显示在边缘的状态信息如FPS数与ms延迟等。
- 碰撞拓展工具：提供更方便的碰撞检查工具。
- 路径帮助器：不同平台的常用可写路径，如截图、存档、配置、日志等路径。
- 文本类序列化器：支持xml与json的序列化与反序列化，支持异步与IO操作。
- 贴图与精灵工具：可以从本地读取图片或将贴图或精灵写入到本地，缩放贴图等。
- 变换拓展：添加与移除监听器，更多的子对象操作。
- 日志记录器：将unity的日志写入本地。

## xLua
该库还在完善阶段，可能会出现不兼容的更新，[查看文档](JxUnity/xLua/README.md)

- 该库的Lua侧使用了[JxCore.LuaSharp](https://github.com/JomiXedYu/JxCode.LuaSharp)作为基础类库，实现了类型系统，关键字，字符串扩展，表扩展工具等，接口与类型命名均参考C#，以方便熟悉C#的开发者使用，  
- 拥有默认的Ticker/Updater，可以在初始化时使用自定义的Ticker/Updater。
- 提供了LuaBehaviour，可以在Lua侧实现组件编程。  