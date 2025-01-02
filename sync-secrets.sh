#!/bin/bash

scriptDir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

exodusSecretsPath="$scriptDir/Resources/Prototypes/ExodusSecrets"
secretsPrototypesPath="$scriptDir/Secrets/Resources/Prototypes"

mapsPath="$scriptDir/Resources/Maps/ExodusSecrets"
secretsMapsPath="$scriptDir/Secrets/Resources/Maps"

localePath="$scriptDir/Resources/Locale/ru-RU/exodus-secrets"
secretsLocalePath="$scriptDir/Secrets/Resources/Locale/ru-RU"

texturesPath="$scriptDir/Resources/Textures/ExodusSecrets"
secretsTexturesPath="$scriptDir/Secrets/Resources/Textures"

process_folder() {
    local targetPath="$1"
    local sourcePath="$2"

    if [ -d "$targetPath" ]; then
        rm -rf "$targetPath"
    fi

    cp -r "$sourcePath" "$targetPath"
}

process_folder "$exodusSecretsPath" "$secretsPrototypesPath"
# process_folder "$mapsPath" "$secretsMapsPath"
process_folder "$localePath" "$secretsLocalePath"
process_folder "$texturesPath" "$secretsTexturesPath"
