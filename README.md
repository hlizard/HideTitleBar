# HideTitleBar


This plugin can automatically hides the Title Bar of Visual Studio main window.

This plugin **automatically works after a solution is loaded**. You can re-display the title bar by **pressing the ALT key**  (like the Hide Main Menu plugin; **VS2019/2022 can only use shortcut Ctrl+1, Ctrl+1**). **Do NOT click the menu item** "Manual Toggle the Visibility of Title Bar  Ctrl+1, Ctrl+1" if you like this way (Click the menu item will switch to manual mode also. In manual mode, you can't use ALT key to control the Visibility of Title Bar).

This plug-in not conflict with Hide Main Menu plugin.


这个插件可以自动为你隐藏Visual Studio主窗口的标题栏。

这个插件在有解决方案加载后自动工作。你可以通过按下ALT键重新显示标题栏（就像Hide Main Menu插件; **VS2019/2022只能使用快捷键Ctrl+1, Ctrl+1**）。如果你喜欢这种方式就不要点击菜单“Manual Toggle the Visibility of Title Bar  Ctrl+1, Ctrl+1”了（使用这个菜单同时就会切换到手动模式，手动模式下就不能使用更方便的ALT键了）。

此插件与Hide Main Menu插件不冲突。

 

 

Version 1.0 - Initial version

Version 1.1 - Added a little extra menu item

Version 1.2 - Click the menu item will switch to manual mode also. In manual mode, you can't use ALT key to control the Visibility of Title Bar.

Version 1.2.1 - Fix a bug

Version 1.2.2 - Disable manual mode for Visual Studio 2010 (Visual Studio 2010实现有时会将底部停靠的窗口也隐藏，考虑使到用ALT键恢复比使用菜单更方便，所以就禁用使用菜单的手动模式了 | The Visual Studio 2010 implementation sometimes hidden the bottom of the main window also, consider using ALT key to restore more convenient than using the menu, i disable the manual mode that using the menu)

2022/07/03: 
**Version 1.3.0** - add support for VS2019

2022/07/05: 
**Version 1.4.0** - add support for VS2022 (https://marketplace.visualstudio.com/items?itemName=dingbo.HideTitleBar-v17) and asynchronous loading

 

 

 
Visual Studio 2010 ~ 2015 see https://marketplace.visualstudio.com/items?itemName=dingbo.HideTitleBar

Visual Studio 2017 see https://marketplace.visualstudio.com/items?itemName=dingbo.HideTitleBar-18430

 

 

Thanks for

https://vlasovstudio.com/visual-commander/extensions.html#hide_title_bar

https://marketplace.visualstudio.com/items?itemName=MatthewJohnsonMSFT.HideMainMenu

I have use some code from them.


# Build

HideTitleBar.csproj —— Visual Studio 2010 ~ 2015 plugin

HideTitleBar.v15.csproj —— Visual Studio 2017/2019 plugin

HideTitleBar.v17.csproj —— Visual Studio 2022 plugin

HideTitleBar.vsix2.csproj —— invalid
