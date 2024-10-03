@echo off
if exist Secrets (
    rm -r Resources/Prototypes/ExodusSecrets
    rm -r Resources/Locale/ru-RU/exodus-secrets
    rm -r Resources/Textures/ExodusSecrets
    rm -r Resources/Maps/ExodusSecrets

    cp -R Secrets/Resources/Prototypes Resources/Prototypes/ExodusSecrets
    cp -R Secrets/Resources/Locale Resources/Locale/ru-RU/exodus-secrets
    cp -R Secrets/Resources/Textures Resources/Textures/ExodusSecrets
    cp -R Secrets/Resources/Maps Resources/Maps/ExodusSecrets
)
