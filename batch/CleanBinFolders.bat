set folder="..\src\DND.Web\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.Web.Plugins\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.Bot\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.Common\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.Domain.CMS\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.Domain.Blog\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.Domain.FlightSearch\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.ApplicationServices\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.ApplicationServices.CMS\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.ApplicationServices.Blog\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.ApplicationServices.FlightSearch\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.DomainServices.CMS\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.DomainServices.Blog\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.DomainServices.FlightSearch\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.Data\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.Data.CMS\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.Data.Blog\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.Data.Identity\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.Infrastructure\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\..\test\DND.UnitTests.CMS\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.UnitsTests.Blog\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.UnitTests.FlightSearch\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.UnitTests.Common\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.IntegrationTestsNUnit\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.IntegrationTestsXUnit\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.UITests\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\DND.TestSetup\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)

set folder="..\..\..\lib\Marvin.Cache.Headers\bin"
cd /d %folder%
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)


