set Configuration=%1
set SolutionPath=%2
set VSWherePath=%3

for /f "usebackq tokens=1* delims=: " %%i in (`%VSWherePath% -latest -requires Microsoft.Component.MSBuild`) do (
  if /i "%%i"=="installationPath" set InstallDir=%%j
)

if exist "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" (  
set MSBuildPath="%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe"
)

if exist %MSBuildPath% (  
%MSBuildPath% "%SolutionPath%\..\src\DND.EFPersistance\DND.EFPersistance.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.UnitOfWork\DND.UnitOfWork.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.Repository\DND.Repository.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.ApplicationServices\DND.ApplicationServices.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.DomainServices\DND.DomainServices.csproj" /p:Configuration=%Configuration% /v:m
)