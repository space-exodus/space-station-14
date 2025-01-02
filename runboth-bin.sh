#!/bin/bash

scriptDir="$(cd "$(dirname "$0")" && pwd)"

contentServerExePath="$scriptDir/bin/Content.Server/Content.Server.exe"
contentClientExePath="$scriptDir/bin/Content.Client/Content.Client.exe"
contentServerPath="$scriptDir/bin/Content.Server/Content.Server"
contentClientPath="$scriptDir/bin/Content.Client/Content.Client"

"$scriptDir/sync-secrets.sh"

source "$scriptDir/functions.sh"

if [ ! -f "$contentServerExePath" ] && [ ! -f "$contentServerPath" ]; then
    build_component "Content.Server" "Content.Exodus.Server"
fi

if [ ! -f "$contentClientExePath" ] && [ ! -f "$contentClientPath" ]; then
    build_component "Content.Client" "Content.Exodus.Client"
fi

run_component "Content.Server"
run_component "Content.Client"
