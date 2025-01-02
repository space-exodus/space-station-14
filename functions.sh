#!/bin/bash

scriptDir="$(cd "$(dirname "$0")" && pwd)"

build_component() {
    local component

    if [ -d "$scriptDir/Secrets" ]; then
        component="Secrets/$1"
    else
        component="$1"
    fi

    echo "$component не найден. Запуск сборки проекта через dotnet build -c Release $component."
    dotnet build -c Release "$component"
}

run_component() {
    local component="$1"

    if [ -f "$scriptDir/bin/$component/$component.exe" ] || [ -f "$scriptDir/bin/$component/$component" ]; then
        if [ "$(uname -s)" = "Windows_NT" ] || [ -n "$MSYSTEM" ]; then
            start "" "$scriptDir/bin/$component/$component.exe"
        else
            "$scriptDir/bin/$component/$component" &
        fi
    else
        echo "Не удалось найти $component после сборки."
    fi
}
