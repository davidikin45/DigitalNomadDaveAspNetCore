Echo.%2 | findstr /C:":">nul && (
    xcopy "%1net47\plugins\*.dll" "%2plugins\" /Y
    xcopy "%1net47\plugins\*.pdb" "%2plugins\" /Y
) || (
    xcopy "%1net47\plugins\*.dll" "%3plugins\" /Y
    xcopy "%1net47\plugins\*.pdb" "%3plugins\" /Y
)