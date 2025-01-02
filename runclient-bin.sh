#!/bin/bash

scriptDir="$(cd "$(dirname "$0")" && pwd)"

"$scriptDir/sync-secrets.sh"

contentExePath="$scriptDir/bin/Content.Client/Content.Client.exe"
contentPath="$scriptDir/bin/Content.Client/Content.Client"

if [ ! -f "$contentExePath" ] && [ ! -f "$contentPath" ]; then
    if [ -d "$scriptDir/Secrets" ]; then
        echo "Content.Client не найден. Запуск сборки проекта через dotnet build -c Release Secrets/Content.Exodus.Client."
        dotnet build -c Release Secrets/Content.Exodus.Client
    else
        echo "Content.Client не найден. Запуск сборки проекта через dotnet build -c Release Content.Client."
        dotnet build -c Release Content.Client
    fi
fi

if [ -f "$contentExePath" ] || [ -f "$contentPath" ]; then
    if [ "$(uname -s)" = "Windows_NT" ] || [ -n "$MSYSTEM" ]; then
        start "" "$contentExePath"
    else
        "$contentPath" &
    fi
else
    echo "Не удалось найти Content.Client после сборки."
fi
