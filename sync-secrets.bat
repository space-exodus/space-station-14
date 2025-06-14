@echo off

set scriptDir=%~dp0

set exodusSecretsPath=%scriptDir%Resources\Prototypes\ExodusSecrets
set secretsPrototypesPath=%scriptDir%Secrets\Resources\Prototypes

set mapsPath=%scriptDir%Resources\Maps\ExodusSecrets
set secretsMapsPath=%scriptDir%Secrets\Resources\Maps

set localePath=%scriptDir%Resources\Locale\ru-RU\exodus-secrets
set secretsLocalePath=%scriptDir%Secrets\Resources\Locale\ru-RU

set texturesPath=%scriptDir%Resources\Textures\ExodusSecrets
set secretsTexturesPath=%scriptDir%Secrets\Resources\Textures

call :ProcessFolder "%exodusSecretsPath%" "%secretsPrototypesPath%"
@REM call :ProcessFolder "%mapsPath%" "%secretsMapsPath%"
call :ProcessFolder "%localePath%" "%secretsLocalePath%"
call :ProcessFolder "%texturesPath%" "%secretsTexturesPath%"

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
