vs2019下编译HideTileBar.v15遇到
Your project does not reference ".NETFramework,Version=v4.6" framework. Add a reference to ".NETFramework,Version=v4.6" in the "TargetFrameworks" property of your project file and then re-run NuGet restore.

成功解决经验：

清理项目， 然后手动删除obj文件夹和bin下Debug和Release文件夹， 然后卸载其余项目， 关闭vs重新打开， 再次生成正常。