#!/bin/bash

scriptDir="$(cd "$(dirname "$0")" && pwd)"

contentServerExePath="$scriptDir/bin/Content.Server/Content.Server.exe"
contentServerPath="$scriptDir/bin/Content.Server/Content.Server"

"$scriptDir/sync-secrets.sh"

source "$scriptDir/functions.sh"

if [ ! -f "$contentServerExePath" ] && [ ! -f "$contentServerPath" ]; then
    build_component "Content.Server" "Content.Exodus.Server"
fi

run_component "Content.Server"
