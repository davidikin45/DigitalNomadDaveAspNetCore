set Configuration=%1
set MSBuildPath="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
set SolutionPath=%cd%

%MSBuildPath% "%SolutionPath%\..\DND.EFPersistance\DND.EFPersistance.csproj" /p:Configuration=%Configuration% /v:n
%MSBuildPath% "%SolutionPath%\..\DND.UnitOfWork\DND.UnitOfWork.csproj" /p:Configuration=%Configuration% /v:n
%MSBuildPath% "%SolutionPath%\..\DND.Repository\DND.Repository.csproj" /p:Configuration=%Configuration% /v:n
%MSBuildPath% "%SolutionPath%\..\DND.ApplicationServices\DND.ApplicationServices.csproj" /p:Configuration=%Configuration% /v:n
%MSBuildPath% "%SolutionPath%\..\DND.DomainServices\DND.DomainServices.csproj" /p:Configuration=%Configuration% /v:n
