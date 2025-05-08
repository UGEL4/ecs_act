@echo off
pushd %~dp0
setlocal
set DOTNET_ROLL_FORWARD=LatestMajor
set MSBUILD_PATH="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin"
set DOTNET_ADDITIONAL_DEPS=%CD%\Jenny\Plugins\additionalDeps

dotnet ./Jenny/Jenny.Generator.Cli.dll gen ^
  --msbuild "%MSBUILD_PATH%\MSBuild.exe" ^
  --property:MSBuildExtensionsPath64="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild"

endlocal
popd