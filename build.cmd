@echo Off
pushd %~dp0
setlocal enabledelayedexpansion

set PROGRAMSROOT=%PROGRAMFILES%
if defined PROGRAMFILES(X86) set PROGRAMSROOT=%PROGRAMFILES(X86)%

set CACHED_NUGET=%LOCALAPPDATA%\NuGet\NuGet.exe
if exist %CACHED_NUGET% goto :CopyNuGet

echo Downloading latest version of NuGet.exe...
if not exist %LOCALAPPDATA%\NuGet @md %LOCALAPPDATA%\NuGet
@powershell -NoProfile -ExecutionPolicy Unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://www.nuget.org/nuget.exe' -OutFile '%CACHED_NUGET%'"

:CopyNuGet
if exist .nuget\nuget.exe goto :Build
if not exist .nuget @md .nuget
@copy %CACHED_NUGET% .nuget\nuget.exe > nul

:Build
dotnet build src/core/core.csproj --configuration release

if %ERRORLEVEL% neq 0 goto :BuildFail

:BuildSuccess
echo.
echo *** BUILD SUCCEEDED ***
goto End

:BuildFail
echo.
echo *** BUILD FAILED ***
goto End

:End
echo.
popd
exit /B %ERRORLEVEL%
