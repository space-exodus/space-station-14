@echo off
setlocal

set scriptDir=%~dp0

if "%1" == "RunComponent" (goto RunComponent)
if "%1" == "BuildComponent" (goto BuildComponent)

:BuildComponent
set component=%2

if exist "%scriptDir%Secrets" (
    set component=Secrets\%3
)

echo %component% не найден. Запуск сборки проекта через dotnet build -c Release %component%.
dotnet build -c Release "%component%"
goto :eof

:RunComponent
set component=%2

if exist "%scriptDir%bin\%component%\%component%.exe" (
    start "" "%scriptDir%bin\%component%\%component%.exe"
) else if exist "%scriptDir%bin\%component%\%component%" (
    start "" "%scriptDir%bin\%component%\%component%"
) else (
    echo Не удалось найти %component% после сборки.
)
goto :eof

endlocal
