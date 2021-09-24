# JxUnity.Framework
 ![](https://img.shields.io/github/license/JomiXedYu/JxCode.CoreLib?style=for-the-badge)

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
  - [UguiEffect](#uguieffect)
  - [UguiUtility](#uguiutility)
  - [Utility](#utility)
  - [xLua](#xlua)

## Basic
该库已进入长期稳定版，[查看文档](JxUnity/Basic/README.md)

基础的单例与Mono单例，状态机等常用设计模板类。
## AssetManagment
该库正在进行功能实现迭代阶段，基本不会出现不兼容的更新，[查看文档](JxUnity/AssetManagment/README.md)

统一的资源读取接口，在Editor中直接读取资源，编译时可以选择放入Resources模式或打包成AssetsBundle，提供了资源分包等工具。
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
## UguiEffect
该库将会不断的更新与迭代，但不会出现不兼容的更新

一些简单的效果实现
## UguiUtility
该库将会不断的更新与迭代，但不会出现不兼容的更新
## Utility
该库将会不断的更新与迭代，但不会出现不兼容的更新，[查看文档](JxUnity/Utility/README.md)

- 自定义截屏
- 碰撞拓展工具
- 几何检测
- FPS显示工具
- 时间表管理
- 贴图与精灵扩展工具
- UGUI事件扩展工具
- 日志记录器

## xLua
该库还在完善阶段，可能会出现不兼容的更新，[查看文档](JxUnity/xLua/README.md)

该库的Lua侧使用了[JxCore.LuaSharp](https://github.com/JomiXedYu/JxCode.LuaSharp)作为基础类库，实现了类型系统，关键字，字符串扩展，表扩展工具等，接口与类型命名均参考C#，以方便熟悉C#的开发者使用，  
提供了LuaBehaviour，可以在Lua侧实现组件编程。  