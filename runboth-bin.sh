#!/bin/bash

scriptDir="$(cd "$(dirname "$0")" && pwd)"

"$scriptDir/sync-secrets.sh"

contentServerExePath="$scriptDir/bin/Content.Server/Content.Server.exe"
contentClientExePath="$scriptDir/bin/Content.Client/Content.Client.exe"
contentServerPath="$scriptDir/bin/Content.Server/Content.Server"
contentClientPath="$scriptDir/bin/Content.Client/Content.Client"

if [ ! -f "$contentServerExePath" ] && [ ! -f "$contentServerPath" ]; then
    if [ -d "$scriptDir/Secrets" ]; then
        echo "Content.Server не найден. Запуск сборки проекта через dotnet build -c Release Secrets/Content.Exodus.Server."
        dotnet build -c Release Secrets/Content.Exodus.Server
    else
        echo "Content.Server не найден. Запуск сборки проекта через dotnet build -c Release Content.Server."
        dotnet build -c Release Content.Server
    fi
fi

if [ ! -f "$contentClientExePath" ] && [ ! -f "$contentClientPath" ]; then
    if [ -d "$scriptDir/Secrets" ]; then
        echo "Content.Client не найден. Запуск сборки проекта через dotnet build -c Release Secrets/Content.Exodus.Client."
        dotnet build -c Release Secrets/Content.Exodus.Client
    else
        echo "Content.Client не найден. Запуск сборки проекта через dotnet build -c Release Content.Client."
        dotnet build -c Release Content.Client
    fi
fi

if [ -f "$contentServerExePath" ] || [ -f "$contentServerPath" ]; then
    if [ "$(uname -s)" = "Windows_NT" ] || [ -n "$MSYSTEM" ]; then
        start "" "$contentServerExePath"
    else
        "$contentServerPath" &
    fi
else
    echo "Не удалось найти Content.Server после сборки."
fi

if [ -f "$contentClientExePath" ] || [ -f "$contentClientPath" ]; then
    if [ "$(uname -s)" = "Windows_NT" ] || [ -n "$MSYSTEM" ]; then
        start "" "$contentClientExePath"
    else
        "$contentClientPath" &
    fi
else
    echo "Не удалось найти Content.Client после сборки."
fi
