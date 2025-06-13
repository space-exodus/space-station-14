#!/bin/bash

scriptDir="$(cd "$(dirname "$0")" && pwd)"

contentClientExePath="$scriptDir/bin/Content.Client/Content.Client.exe"
contentClientPath="$scriptDir/bin/Content.Client/Content.Client"

"$scriptDir/sync-secrets.sh"

source "$scriptDir/functions.sh"

if [ ! -f "$contentClientExePath" ] && [ ! -f "$contentClientPath" ]; then
    build_component "Content.Client" "Content.Exodus.Client"
fi

run_component "Content.Client"
