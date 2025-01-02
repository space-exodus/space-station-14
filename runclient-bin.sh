#!/bin/sh

scriptDir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

"$scriptDir/sync-secrets.sh"

contentExePath="$scriptDir/bin/Content.Client/Content.Client.exe"

if [ -f "$contentExePath" ]; then
    if [[ "$OSTYPE" == "msys" || "$OSTYPE" == "win32" ]]; then
        start "" "$contentExePath"
    else
        "$contentExePath" &
    fi
else
    echo "EXE-файл не найден. Запуск сборки проекта с помощью 'dotnet build -c Release'."
    cd "$scriptDir"
    dotnet build -c Release
fi
