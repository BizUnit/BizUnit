@Echo Off

set PATH=%PATH%;"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE"

MSBuild.exe src\BizUnit.Core\BizUnit.Core.csproj /t:Build /p:Configuration="Release"
if %errorlevel% neq 0 exit /b %errorlevel%
MSBuild.exe src\BizUnit.Core\BizUnit.Core.csproj /t:Build /p:Configuration="Release-Net45"
if %errorlevel% neq 0 exit /b %errorlevel%
MSBuild.exe src\BizUnit.Core\BizUnit.Core.csproj /t:Build /p:Configuration="Release-Net46"
if %errorlevel% neq 0 exit /b %errorlevel%


MSBuild.exe src\BizUnit.TestSteps\BizUnit.TestSteps.csproj /t:Build /p:Configuration="Release"
if %errorlevel% neq 0 exit /b %errorlevel%
MSBuild.exe src\BizUnit.TestSteps\BizUnit.TestSteps.csproj /t:Build /p:Configuration="Release-Net45"
if %errorlevel% neq 0 exit /b %errorlevel%
MSBuild.exe src\BizUnit.TestSteps\BizUnit.TestSteps.csproj /t:Build /p:Configuration="Release-Net46"
if %errorlevel% neq 0 exit /b %errorlevel%


MSBuild.exe src\BizUnit.TestSteps.Azure\BizUnit.TestSteps.Azure.csproj /t:Build /p:Configuration="Release"
if %errorlevel% neq 0 exit /b %errorlevel%
MSBuild.exe src\BizUnit.TestSteps.Azure\BizUnit.TestSteps.Azure.csproj /t:Build /p:Configuration="Release-Net45"
if %errorlevel% neq 0 exit /b %errorlevel%
MSBuild.exe src\BizUnit.TestSteps.Azure\BizUnit.TestSteps.Azure.csproj /t:Build /p:Configuration="Release-Net46"
if %errorlevel% neq 0 exit /b %errorlevel%


MSBuild.exe src\BizUnit.Core\BizUnit.Core.csproj /t:Package /p:Configuration="Release"
if %errorlevel% neq 0 exit /b %errorlevel%
MSBuild.exe src\BizUnit.TestSteps\BizUnit.TestSteps.csproj /t:Package /p:Configuration="Release"
if %errorlevel% neq 0 exit /b %errorlevel%
MSBuild.exe src\BizUnit.TestSteps.Azure\BizUnit.TestSteps.Azure.csproj /t:Package /p:Configuration="Release"
if %errorlevel% neq 0 exit /b %errorlevel%


mkdir NuGet
copy src\BizUnit.Core\NuGet\BizUnit.Core.5.0.3.nupkg .\NuGet\BizUnit.Core.5.0.3.nupkg /Y
if %errorlevel% neq 0 exit /b %errorlevel%
copy src\BizUnit.TestSteps\NuGet\BizUnit.TestSteps.5.0.3.nupkg .\NuGet\BizUnit.TestSteps.5.0.3.nupkg /Y
if %errorlevel% neq 0 exit /b %errorlevel%
copy src\BizUnit.TestSteps.Azure\NuGet\BizUnit.TestSteps.Azure.1.0.1.nupkg .\NuGet\BizUnit.TestSteps.Azure.1.0.1.nupkg /Y
if %errorlevel% neq 0 exit /b %errorlevel%

echo
echo S u c c e s s
echo
pause
