@echo off
set scriptDir=%~dp0

call ./sync-secrets.bat

set contentExePath=%scriptDir%bin\Content.Server\Content.Server.exe

if exist "%contentExePath%" (
    start "" "%contentExePath%"
) else (
    echo EXE-файл не найден. Запуск сборки проекта с помощью "dotnet build -c Release".
    cd /d "%scriptDir%"
    dotnet build -c Release
)
