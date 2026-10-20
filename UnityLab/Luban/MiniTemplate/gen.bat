@echo off
set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.

:: Unity 的 Assets 路径（从 MiniTemplate 往上退两层到达 UnityLab 工程目录）
set UNITY_ASSETS=..\..\Assets

dotnet %LUBAN_DLL% ^
    -t all ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=%UNITY_ASSETS%\Data\Runtime\DataTables ^
    -x outputDataDir=%UNITY_ASSETS%\Data\Resources\Config

pause