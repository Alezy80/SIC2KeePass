set Sic=SafeInCloudImp
set SicTmp=SafeInCloudImp~
md %SicTmp%
xcopy /y /d %Sic%\*.cs %SicTmp%\
xcopy /y /d %Sic%\*.csproj %SicTmp%\
xcopy /y /d %Sic%\*.snk %SicTmp%\
xcopy /y /d /i %Sic%\Properties %SicTmp%\Properties
xcopy /y /d /i %Sic%\Resources %SicTmp%\Resources
KeePass\KeePass.exe --plgx-create "%~dp0%SicTmp%"
del %Sic%.plgx
ren %SicTmp%.plgx %Sic%.plgx
xcopy /y %Sic%.plgx KeePass\plugins\
rd /s /q %SicTmp%