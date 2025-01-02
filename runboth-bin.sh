#!/bin/sh

scriptDir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

contentServerExePath="$scriptDir/bin/Content.Server/Content.Server.exe"
contentClientExePath="$scriptDir/bin/Content.Client/Content.Client.exe"
contentServerPath="$scriptDir/bin/Content.Server/Content.Server"
contentClientPath="$scriptDir/bin/Content.Client/Content.Client"

if [ ! -f "$contentServerExePath" ] && [ ! -f "$contentServerPath" ]; then
    echo "Content.Server не найден. Запуск сборки проекта через dotnet build -c Release Content.Server."
    dotnet build -c Release Content.Server
fi

if [ ! -f "$contentClientExePath" ] && [ ! -f "$contentClientPath" ]; then
    echo "Content.Client не найден. Запуск сборки проекта через dotnet build -c Release Content.Client."
    dotnet build -c Release Content.Client
fi

if [ -f "$contentServerExePath" ] || [ -f "$contentServerPath" ]; then
    if [[ "$OSTYPE" == "msys" || "$OSTYPE" == "win32" ]]; then
        start "" "$contentServerExePath"
    else
        "$contentServerPath" &
    fi
else
    echo "Не удалось найти Content.Server после сборки."
fi

if [ -f "$contentClientExePath" ] || [ -f "$contentClientPath" ]; then
    if [[ "$OSTYPE" == "msys" || "$OSTYPE" == "win32" ]]; then
        start "" "$contentClientExePath"
    else
        "$contentClientPath" &
    fi
else
    echo "Не удалось найти Content.Client после сборки."
fi
