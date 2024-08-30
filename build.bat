@echo off
set configuration=%1

if exist Secrets (
    dotnet build -c %configuration% Secrets
) else (
    dotnet build -c %configuration%
)
