@echo off
set scriptDir=%~dp0

call ./sync-secrets.bat

set contentExePath=%scriptDir%bin\Content.Server\Content.Server.exe

if not exist "%contentExePath%" (
    if exist "%scriptDir%Secrets" (
        echo Content.Server.exe не найден. Запуск сборки проекта через dotnet build -c Release Secrets/Content.Exodus.Server.
        dotnet build -c Release Secrets/Content.Exodus.Server
    ) else (
        echo Content.Server.exe не найден. Запуск сборки проекта через dotnet build -c Release Content.Server.
        dotnet build -c Release Content.Server
    )
)

if exist "%contentExePath%" (
    start "" "%contentExePath%"
) else (
    echo Не удалось найти Content.Server.exe после сборки.
)
