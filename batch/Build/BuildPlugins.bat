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
%MSBuildPath% "%SolutionPath%\..\src\DND.ApplicationServices\DND.ApplicationServices.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.ApplicationServices.Blog\DND.ApplicationServices.Blog.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.ApplicationServices.CMS\DND.ApplicationServices.CMS.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.ApplicationServices.FlightSearch\DND.ApplicationServices.FlightSearch.csproj" /p:Configuration=%Configuration% /v:m
 
%MSBuildPath% "%SolutionPath%\..\src\DND.DomainServices.Blog\DND.DomainServices.Blog.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.DomainServices.CMS\DND.DomainServices.CMS.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.DomainServices.FlightSearch\DND.DomainServices.FlightSearch.csproj" /p:Configuration=%Configuration% /v:m

%MSBuildPath% "%SolutionPath%\..\src\DND.Data\DND.Data.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.Data.Blog\DND.Data.Blog.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.Data.CMS\DND.Data.CMS.csproj" /p:Configuration=%Configuration% /v:m
)