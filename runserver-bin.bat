@echo off
setlocal

set scriptDir=%~dp0

call %scriptDir%sync-secrets.bat

if not exist "%scriptDir%bin\Content.Server\Content.Server.exe" if not exist "%scriptDir%bin\Content.Server\Content.Server" (
    call "%scriptDir%functions.bat" BuildComponent Content.Server Content.Exodus.Server
)

call "%scriptDir%functions.bat" RunComponent Content.Server

endlocal
