@echo off

set scriptDir=%~dp0

set protoPath=%scriptDir%Resources\Prototypes\ExodusSecrets
set secretsProtoPath=%scriptDir%Secrets\Resources\Prototypes

set mapsPath=%scriptDir%Resources\Maps\ExodusSecrets
set secretsMapsPath=%scriptDir%Secrets\Resources\Maps

set localePath=%scriptDir%Resources\Locale\ru-RU\exodus-secrets
set secretsLocalePath=%scriptDir%Secrets\Resources\Locale\ru-RU

set texturesPath=%scriptDir%Resources\Textures\ExodusSecrets
set secretsTexturesPath=%scriptDir%Secrets\Resources\Textures

set audioPath=%scriptDir%Resources\Audio\ExodusSecrets
set secretsAudioPath=%scriptDir%Secrets\Resources\Audio

call :ProcessFolder "%protoPath%" "%secretsProtoPath%"
@REM call :ProcessFolder "%mapsPath%" "%secretsMapsPath%"
call :ProcessFolder "%localePath%" "%secretsLocalePath%"
call :ProcessFolder "%texturesPath%" "%secretsTexturesPath%"
call :ProcessFolder "%audioPath%" "%secretsAudioPath%"

exit /b

:ProcessFolder
set targetPath=%1
set sourcePath=%2

if exist "%targetPath%" (
    rmdir /s /q "%targetPath%"
)

if exist "%sourcePath%" (
    xcopy /e /i /q "%sourcePath%" "%targetPath%" >nul
)
