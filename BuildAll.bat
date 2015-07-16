set PATH=%PATH%;"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE"

C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe Build.proj /t:Clean
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe Build.proj /t:Build /p:BuildConfiguration=Release-Net45
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe Build.proj /t:Build /p:BuildConfiguration=Release-Net46
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe Build.proj /t:BuildAndTest
pause
