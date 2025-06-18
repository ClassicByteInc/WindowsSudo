@echo off
setlocal
 
:: 检查是否提供了命令参数
if "%~1"=="" (
    echo 错误: 请提供要执行的命令。
    echo 用法: sudo [命令] [参数...]
    exit /b 1
)
 
:: 保存原始命令行
set "fullCommand=%*"
 
:: 检查UAC状态
reg query HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v EnableLUA | find "0x1" >nul
if %errorlevel% equ 0 (
    set "uacEnabled=true"
) else (
    set "uacEnabled=false"
)
 
:: 检查是否是PowerShell命令
echo %1 | findstr /i /c:"powershell" /c:"pwsh" >nul
if %errorlevel% equ 0 (
    :: 是PowerShell命令，提取参数部分
    set "psCommand="
    shift
    :psLoop
    if not "%~1"=="" (
        set "psCommand=%psCommand% %~1"
        shift
        goto psLoop
    )
    
    :: 移除开头的空格
    for /f "tokens=* delims= " %%a in ("%psCommand%") do set "psCommand=%%a"
    
    :: 使用PowerShell启动提升的PowerShell会话
    if "%uacEnabled%"=="true" (
        powershell -Command "$psi = New-Object System.Diagnostics.ProcessStartInfo; $psi.FileName = 'powershell.exe'; $psi.Arguments = '-NoProfile -Command ""%psCommand%""'; $psi.Verb = 'runas'; [System.Diagnostics.Process]::Start($psi)"
    ) else (
        powershell -Command "Start-Process -FilePath 'powershell.exe' -ArgumentList '-NoProfile -Command ""%psCommand%""' -Verb RunAs -Wait"
    )
    exit /b
)
 
:: 检查是否是CMD内部命令
set "internalCommands=cd dir type echo mkdir rmdir copy move del ren cls ver help exit"
echo %internalCommands% | findstr /i /b /c:"%1 " >nul
if %errorlevel% equ 0 (
    :: 是CMD内部命令，在提升的CMD中执行
    if "%uacEnabled%"=="true" (
        powershell -Command "$psi = New-Object System.Diagnostics.ProcessStartInfo; $psi.FileName = 'cmd.exe'; $psi.Arguments = '/c %fullCommand%'; $psi.Verb = 'runas'; [System.Diagnostics.Process]::Start($psi)"
    ) else (
        powershell -Command "Start-Process -FilePath 'cmd.exe' -ArgumentList '/c %fullCommand%' -Verb RunAs -Wait"
    )
    exit /b
)
 
:: 处理普通命令
if "%uacEnabled%"=="true" (
    powershell -Command "$psi = New-Object System.Diagnostics.ProcessStartInfo; $psi.FileName = '%1'; $psi.Arguments = '%fullCommand:*%1=%%'; $psi.Verb = 'runas'; [System.Diagnostics.Process]::Start($psi)"
) else (
    powershell -Command "Start-Process -FilePath '%1' -ArgumentList '%fullCommand:*%1=%%' -Verb RunAs -Wait"
)

endlocal    