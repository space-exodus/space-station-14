@echo off
set scriptDir=%~dp0

call ./sync-secrets.bat

set contentExePath=%scriptDir%bin\Content.Client\Content.Client.exe

if not exist "%contentExePath%" (
    if exist "%scriptDir%Secrets" (
        echo Content.Client.exe не найден. Запуск сборки проекта через dotnet build -c Release Secrets/Content.Exodus.Client.
        dotnet build -c Release Secrets/Content.Exodus.Client
    ) else (
        echo Content.Client.exe не найден. Запуск сборки проекта через dotnet build -c Release Content.Client.
        dotnet build -c Release Content.Client
    )
)

if exist "%contentExePath%" (
    start "" "%contentExePath%"
) else (
    echo Не удалось найти Content.Client.exe после сборки.
)