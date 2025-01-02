@echo off
set scriptDir=%~dp0

call sync-secrets.bat

set contentServerExePath=%scriptDir%bin\Content.Server\Content.Server.exe
set contentClientExePath=%scriptDir%bin\Content.Client\Content.Client.exe

if not exist "%contentServerExePath%" (
    if exist "%scriptDir%Secrets" (
        echo Content.Server.exe не найден. Запуск сборки проекта через dotnet build -c Release Secrets/Content.Exodus.Server.
        dotnet build -c Release Secrets/Content.Exodus.Server
    ) else (
        echo Content.Server.exe не найден. Запуск сборки проекта через dotnet build -c Release Content.Server.
        dotnet build -c Release Content.Server
    )
)

if not exist "%contentClientExePath%" (
    if exist "%scriptDir%Secrets" (
        echo Content.Client.exe не найден. Запуск сборки проекта через dotnet build -c Release Secrets/Content.Exodus.Client.
        dotnet build -c Release Secrets/Content.Exodus.Client
    ) else (
        echo Content.Client.exe не найден. Запуск сборки проекта через dotnet build -c Release Content.Client.
        dotnet build -c Release Content.Client
    )
)

if exist "%contentServerExePath%" (
    start "" "%contentServerExePath%"
) else (
    echo Не удалось найти Content.Server.exe после сборки.
)

if exist "%contentClientExePath%" (
    start "" "%contentClientExePath%"
) else (
    echo Не удалось найти Content.Client.exe после сборки.
)
