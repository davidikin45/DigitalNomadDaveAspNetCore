set Configuration=%1
set MSBuildPath="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
set SolutionPath=%2

%MSBuildPath% "%SolutionPath%\..\src\DND.EFPersistance\DND.EFPersistance.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.UnitOfWork\DND.UnitOfWork.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.Repository\DND.Repository.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.ApplicationServices\DND.ApplicationServices.csproj" /p:Configuration=%Configuration% /v:m
%MSBuildPath% "%SolutionPath%\..\src\DND.DomainServices\DND.DomainServices.csproj" /p:Configuration=%Configuration% /v:m
