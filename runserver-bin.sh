#!/bin/bash

scriptDir="$(cd "$(dirname "$0")" && pwd)"

"$scriptDir/sync-secrets.sh"

contentExePath="$scriptDir/bin/Content.Server/Content.Server.exe"
contentPath="$scriptDir/bin/Content.Server/Content.Server"

if [ ! -f "$contentExePath" ] && [ ! -f "$contentPath" ]; then
    if [ -d "$scriptDir/Secrets" ]; then
        echo "Content.Server не найден. Запуск сборки проекта через dotnet build -c Release Secrets/Content.Exodus.Server."
        dotnet build -c Release Secrets/Content.Exodus.Server
    else
        echo "Content.Server не найден. Запуск сборки проекта через dotnet build -c Release Content.Server."
        dotnet build -c Release Content.Server
    fi
fi

if [ -f "$contentExePath" ] || [ -f "$contentPath" ]; then
    if [ "$(uname -s)" = "Windows_NT" ] || [ -n "$MSYSTEM" ]; then
        start "" "$contentExePath"
    else
        "$contentPath" &
    fi
else
    echo "Не удалось найти Content.Server после сборки."
fi
