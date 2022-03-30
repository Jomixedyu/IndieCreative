# JxUnity.Framework
 ![](https://img.shields.io/github/license/JomiXedYu/JxCode.CoreLib?style=for-the-badge)

Unity3d开发库，每个模块可独立使用。

## Content
- [JxUnity.Framework](#jxunityframework)
  - [Partten](#Partten)
  - [Jxugui](#Jxugui)
  - [Languages](#Languages)
  - [JxUGUI](#jxugui)
  - [Localization](#localization)
  - [Media](#media)
  - [Mods](#mods)
  - [Pool](#pool)
  - [Platforms](#Platforms)
  - [Platforms.Steam](#Platforms.Steam)
  - [Procedures](#procedures)
  - [ResDB](#resdb)
  - [SaveDatas](#Savedatas)
  - [Subtitles](#subtitles)
  - [Timers](#Timers)
  - [Utility](#utility)
  - [Ugui](#Ugui)
  - [xLua](#xlua)

## Partten
该库已进入长期稳定版，[查看文档](JxUnity.Partten/README.md)

单例，状态机，事件系统等常用模式。


## JxUGUI
该库已进入长期稳定版，[查看文档](JxUnity.Jxugui/README.md)

通过C#特性来自动绑定元素，脱离了MonoBehaviour，可以轻松在其他框架下如ILRuntime等环境中运行，每个顶级UI都是一个单例，增加了显示栈，焦点消息等功能。

## Languages
该库已进入长期稳定版，[查看文档](JxUnity.Languages/README.md)

使unity支持多语言。

## Media
该库已进入长期稳定版，[查看文档](JxUnity.Media/README.md)

实现了音效池与播放器等组件，解决音频截断，以及实现音乐淡入淡出等功能。

## Mods
迭代中，[查看文档](JxUnity.Mods/README.md)

让你的游戏支持Mods
## Pool
该库已进入长期稳定版，[查看文档](JxUnity/Pool/README.md)

为C#对象、mono对象、List对象提供对象池。

## Platforms
该库迭代中

提供版本号管理，平台实现映射

## Platforms.Steam
该库迭代中，需要前置 JxUnity.Platforms，Steamworks.Net

Steam平台实现
## Proceduress
该库已进入长期稳定版，[查看文档](JxUnity.Procedures/README.md)

为游戏提供一个易用的全局状态机，只须继承，状态自动绑定。


## ResDB
迭代中

资源打包与加载工具

## SaveDatas
迭代中

可寻址的key/value型游戏存档读档工具

## Subtitles
该库已进入长期稳定版，[查看文档](JxUnity.Subtitles/README.md)

支持本地化的字幕系统。

## Timers
该库已进入长期稳定版，[查看文档](JxUnity.Timer/README.md)

待办事件表，跑马灯计时器，倒计时器

## Ugui
Ugui拓展，包含拖拽，序列图功能，打字机、闪烁等特效，与扩展工具。

## Utility
该库将会不断的更新与迭代，但不会出现不兼容的更新，[查看文档](JxUnity/Utility/README.md)

一个类就是一个工具，互相之间没有依赖，可以随意添加与删除。

## xLua
该库还在完善阶段，可能会出现不兼容的更新，[查看文档](JxUnity/xLua/README.md)

- 该库的Lua侧使用了[JxCore.LuaSharp](https://github.com/JomiXedYu/JxCode.LuaSharp)作为基础类库，实现了类型系统，关键字，字符串扩展，表扩展工具等，接口与类型命名均参考C#，以方便熟悉C#的开发者使用，  
- 拥有默认的Ticker/Updater，可以在初始化时使用自定义的Ticker/Updater。
- 提供了LuaBehaviour，可以在Lua侧实现组件编程。  