
# WindowsSudo

一个在 Windows 平台下模拟 Linux `sudo` 行为的命令行工具，支持以管理员权限运行可执行文件或 shell 命令，简化提权操作。

## 特性

- 支持以管理员权限运行任意可执行文件（如 `.exe`）。
- 支持以管理员权限执行 shell 内置命令（自动适配 PowerShell 或 CMD）。
- 自动检测并使用当前或用户默认 shell。
- 支持 .NET 9，AOT 发布，启动速度快。

## 安装

使用```dotnet```命令行工具安装
```
dotnet tool install sudo
```

或者在该仓库的[Release](https://github.com/ClassicByteInc/WindowsSudo/releases)中找到

## 使用方法

### 以管理员权限运行可执行文件
# WindowsSudo

一个在 Windows 平台下模拟 Linux `sudo` 行为的命令行工具，支持以管理员权限运行可执行文件或 shell 命令，简化提权操作。

## 特性

- 支持以管理员权限运行任意可执行文件（如 `.exe`）。
- 支持以管理员权限执行 shell 内置命令（自动适配 PowerShell 或 CMD）。
- 自动检测并使用当前或用户默认 shell。
- 支持 .NET 9，AOT 发布，启动速度快。

## 安装

使用```dotnet```命令行工具安装
### 以管理员权限执行 shell 命令

- PowerShell 下：
- CMD 下：
### 查看帮助
## 配置默认 Shell

可通过修改用户目录下 `.sudo/DefaultShell.cfg` 文件，指定默认 shell 路径。

## 依赖

- .NET 9 SDK
- Windows 10/11

## 许可

MIT License

---

> 本项目灵感来源于 Linux 的 sudo，旨在提升 Windows 下命令行提权体验。

## 使用方法

### 以管理员权限运行可执行文件

```
sudo notepad.exe C:\Windows\System32\drivers\etc\hosts
```

### 以管理员权限执行 shell 命令

- PowerShell 下：
```
  sudo Copy-Item "C:\a.txt" "D:\b.txt"
```
- CMD 下：
```
  sudo copy "C:\a.txt" "D:\b.txt"
```

### 查看帮助

```
sudo

```

## 配置默认 Shell

可通过修改用户目录下 `.sudo/DefaultShell.cfg` 文件，指定默认 shell 路径。

## 依赖

- .NET 9 SDK
- Windows 10/11

## 许可

MIT License

---

> 本项目灵感来源于 Linux 的 sudo，旨在提升 Windows 下命令行提权体验。