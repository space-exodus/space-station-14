#!/bin/bash

scriptDir="$(cd "$(dirname "$0")" && pwd)"

protoPath="$scriptDir/Resources/Prototypes/ExodusSecrets"
secretsProtoPath="$scriptDir/Secrets/Resources/Prototypes"

mapsPath="$scriptDir/Resources/Maps/ExodusSecrets"
secretsMapsPath="$scriptDir/Secrets/Resources/Maps"

localePath="$scriptDir/Resources/Locale/ru-RU/exodus-secrets"
secretsLocalePath="$scriptDir/Secrets/Resources/Locale/ru-RU"

texturesPath="$scriptDir/Resources/Textures/ExodusSecrets"
secretsTexturesPath="$scriptDir/Secrets/Resources/Textures"

audioPath="$scriptDir/Resources/Audio/ExodusSecrets"
secretsAudioPath="$scriptDir/Secrets/Resources/Audio"

process_folder() {
    local targetPath="$1"
    local sourcePath="$2"

    if [ -d "$targetPath" ]; then
        rm -rf "$targetPath"
    fi

    if [ -d "$sourcePath" ]; then
        cp -r "$sourcePath" "$targetPath"
    fi
}

process_folder "$protoPath" "$secretsProtoPath"
# process_folder "$mapsPath" "$secretsMapsPath"
process_folder "$localePath" "$secretsLocalePath"
process_folder "$texturesPath" "$secretsTexturesPath"
process_folder "$audioPath" "$secretsAudioPath"
