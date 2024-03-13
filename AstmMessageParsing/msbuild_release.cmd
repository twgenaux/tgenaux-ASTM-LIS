@echo OFF

REM https://learn.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2015/msbuild/msbuild?view=vs-2015

REM Visual Studio MSBUILD path
@SET MSBUILD="C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"

@SET FLAGS=-t:Rebuild -p:Configuration=Debug -noConsoleLogger -fileLoggerParameters:LogFile=msbuild_debug.log;Append;Verbosity=normal;Encoding=UTF-8

@del .\msbuild_release.log

REM Must be build based on dependencies. Build with Visual Studio and copy the order.
@%MSBUILD% ..\AstmCommon\AstmCommon.csproj %FLAGS%
@%MSBUILD% .\AstmMessageParsing\AstmMessageParsing.csproj %FLAGS%

pause
