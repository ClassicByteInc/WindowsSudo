@echo off
setlocal

:: 检查是否以管理员权限运行
if "%~1"=="" (
    echo Usage: sudo [command] [arguments...]
    exit /b 1
)
 
:: 保存原始命令和参数
set "command=%~1"
shift
set "arguments="
:loop
if not "%~1"=="" (
    set "arguments=%arguments% %~1"
    shift
    goto loop
)
 
:: 检查UAC状态
reg query HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v EnableLUA | find "0x1" >nul
if %errorlevel% equ 0 (
    :: UAC已启用，使用PowerShell创建提升的进程
    powershell -Command "$psi = New-Object System.Diagnostics.ProcessStartInfo; $psi.FileName = '%command%'; $psi.Arguments = '%arguments%'; $psi.Verb = 'runas'; [System.Diagnostics.Process]::Start($psi)"
) else (
    :: UAC未启用，直接以管理员权限运行
    powershell -Command "Start-Process -FilePath '%command%' -ArgumentList '%arguments%' -Verb RunAs -Wait"
)
 
endlocal    