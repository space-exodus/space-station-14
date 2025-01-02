@echo off
set scriptDir=%~dp0

call sync-secrets.bat

set contentServerExePath=%scriptDir%bin\Content.Server\Content.Server.exe
set contentClientExePath=%scriptDir%bin\Content.Client\Content.Client.exe

if not exist "%contentServerExePath%" (
    echo Content.Server.exe не найден. Запуск сборки проекта через dotnet build -c Release Content.Server.
    dotnet build -c Release Content.Server
)

if not exist "%contentClientExePath%" (
    echo Content.Client.exe не найден. Запуск сборки проекта через dotnet build -c Release Content.Client.
    dotnet build -c Release Content.Client
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
