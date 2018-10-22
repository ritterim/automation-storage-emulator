@echo Off
pushd %~dp0
setlocal enabledelayedexpansion

rmdir /s /q "artifacts"

:Build
echo.
echo *** STARTING BUILD ***
echo.

dotnet build src/core/core.csproj --configuration Release
if %ERRORLEVEL% neq 0 goto :BuildFail

echo.
echo *** BUILD SUCCEEDED ***
echo.

echo.
echo *** STARTING TESTS ***
echo.

dotnet test tests/core.tests/core.tests.csproj --configuration Release
if %ERRORLEVEL% neq 0 goto :TestFail

echo.
echo *** TESTS SUCCEEDED ***
echo.

echo.
echo *** STARTING PACK ***
echo.

dotnet pack src/core/core.csproj --configuration Release --no-build --output ../../artifacts
if %ERRORLEVEL% neq 0 goto :PackFail

echo.
echo *** PACK SUCCEEDED ***
echo.
goto End

:BuildFail
echo.
echo *** BUILD FAILED ***
goto End

:TestFail
echo.
echo *** TEST FAILED ***
goto End

:PackFail
echo.
echo *** PACK FAILED ***
goto End

:End
echo.
popd
exit /B %ERRORLEVEL%
