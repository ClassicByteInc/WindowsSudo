using System.ComponentModel;
using System.Diagnostics;

namespace WindowsSudo;

public class Program
{
	public static string[] Comlets { get; set; } = [];
	private static readonly string[] cmdComlets = new[]
			{
	"assoc", "break", "call", "cd", "chdir", "cls", "color", "copy", "date", "del", "dir",
	"echo", "endlocal", "erase", "exit", "for", "ftype", "goto", "if", "md", "mkdir", "move",
	"path", "pause", "popd", "prompt", "pushd", "rd", "rem", "ren", "rename", "rmdir", "set",
	"setlocal", "shift", "start", "time", "title", "type", "ver", "verify", "vol"
	};

	public static int Main(string[] args)
	{
		if (args.Length == 0)
		{
			DisplayHelp();
			return 1;
		}
		var command = args[0];
		// 新增方法：在PATH环境变量中查找可执行文件
		var exePath = FindExeInPath(command);
		// 如果在Path中找到
		if (exePath != null)
		{
			RunAsExeFile(exePath, [.. args.Skip(1)]);
			return 0;
		}
		// 否则在当前目录查找
		if (File.Exists(command))
		{
			RunAsExeFile(command, [.. args.Skip(1)]);
			return 0;
		}
		else
		{
			RunAsComlet(command, args.Skip(1).ToArray());
			return 0;
		}
	}

	public static string? FindExeInPath(string command)
	{
		var pathEnv = Environment.GetEnvironmentVariable("PATH");
		if (string.IsNullOrEmpty(pathEnv))
			return null;

		var paths = pathEnv.Split(Path.PathSeparator);
		foreach (var path in paths)
		{
			var candidate = Path.Combine(path, command);
			if (File.Exists(candidate))
			{
				return candidate;
			}
			// 补全.exe后缀
			if (!command.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
			{
				var candidateExe = Path.Combine(path, command + ".exe");
				if (File.Exists(candidateExe))
				{
					return candidateExe;
				}
			}
		}
		return null;
	}

	public static void DisplayHelp()
	{
		Console.WriteLine("WindowsSudo - Windows 下的 sudo 工具");
		Console.WriteLine();
		Console.WriteLine("用法:");
		Console.WriteLine("  sudo <可执行文件> [参数 ...]");
		Console.WriteLine("    以管理员权限运行指定的可执行文件，并传递参数。");
		Console.WriteLine();
		Console.WriteLine("  sudo <命令> [参数 ...]");
		Console.WriteLine("    以管理员权限在当前 shell 执行内置命令或脚本。");
		Console.WriteLine();
		Console.WriteLine("示例:");
		Console.WriteLine("  sudo notepad.exe C:\\Windows\\System32\\drivers\\etc\\hosts");
		Console.WriteLine("  sudo copy \"C:\\file.txt\" \"D:\\file.txt\"");
		Console.WriteLine("  sudo Copy-Item \"C:\\a.txt\" \"D:\\a.txt\"");
		Console.WriteLine();
		Console.WriteLine("支持的 shell 命令会根据当前 shell 类型自动适配（cmd/powershell）。");
		Console.WriteLine("如需更改默认 shell，可使用 SetUserDefaultShell 方法。");
		Console.WriteLine();
		Console.WriteLine("项目地址: https://github.com/ClassicByteInc/WindowsSudo");
	}

	public static void RunAsExeFile(string fileName, string[]? args = null)
	{
		var psi = new ProcessStartInfo
		{
			FileName = fileName,
			UseShellExecute = true,
			WorkingDirectory = Directory.GetCurrentDirectory(),
			Verb = "runas" // 触发UAC提权
		};

		// 拼接剩余参数
		if (args != null && args.Length > 0)
		{
			psi.Arguments = string.Join(" ", args.Select(a => $"\"{a}\""));
		}

		try
		{
			Process.Start(psi);
		}
		catch (Win32Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			// 用户取消UAC或其他错误
			Console.Error.WriteLine($"启动失败: {ex.Message}");
			Console.ResetColor();
		}
	}

	public static void ReadComlets()
	{
		string shellPath = GetUserDefaultShell().Trim();
		List<string> comlets = [];

		if (shellPath.EndsWith("pwsh.exe", StringComparison.OrdinalIgnoreCase) ||
		shellPath.EndsWith("powershell.exe", StringComparison.OrdinalIgnoreCase))
		{
			// 获取 PowerShell 支持的所有命令
			var psi = new ProcessStartInfo
			{
				FileName = shellPath,
				Arguments = "-NoProfile -Command \"Get-Command | Select-Object -ExpandProperty Name\"",
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};
			try
			{
				using var proc = Process.Start(psi);
				if (proc != null)
				{
					while (!proc.StandardOutput.EndOfStream)
					{
						var line = proc.StandardOutput.ReadLine();
						if (!string.IsNullOrWhiteSpace(line))
							comlets.Add(line.Trim());
					}
					proc.WaitForExit();
				}
			}
			catch
			{
				// 忽略异常
			}
		}
		else if (shellPath.EndsWith("cmd.exe", StringComparison.OrdinalIgnoreCase))
		{
			// cmd 支持的内置命令（部分常用）
			comlets.AddRange(cmdComlets);
		}

		Comlets = [.. comlets];
	}

	public static string GetUserDefaultShell()
	{
		var configFile = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
			".sudo",
			"DefaultShell.cfg"
			);
		if (File.Exists(configFile))
		{
			return File.ReadAllText(configFile, System.Text.Encoding.UTF8);
		}
		else if (GetCurrentShell() != "")
		{
			return GetCurrentShell();
		}
		else
		{
			string? shell;
			if ((shell = FindExeInPath("pwsh.exe")) is not null)
			{
				return shell;
			}
			else if ((shell = FindExeInPath("powershell.exe")) is not null)
			{
				return shell;
			}
			else
			{
				return "cmd.exe";
			}
		}
	}

	public static string GetCurrentShell()
	{
		// 优先从环境变量获取父进程 shell
		try
		{
			using var process = Process.GetCurrentProcess();
			var parentId = 0;
			using (var searcher = new System.Management.ManagementObjectSearcher(
				"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = " + process.Id))
			{
				foreach (var obj in searcher.Get())
				{
					parentId = Convert.ToInt32(obj["ParentProcessId"]);
					break;
				}
			}
			if (parentId > 0)
			{
				using var parentProc = Process.GetProcessById(parentId);
				var parentExe = parentProc.MainModule?.FileName;
				if (!string.IsNullOrEmpty(parentExe))
					return parentExe;
			}
		}
		catch
		{
			// 忽略异常，回退到默认 shell
		}
		// 回退到用户默认 shell
		return "";
	}

	public static void SetUserDefaultShell(string shellName)
	{
		var configFile = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
			".sudo",
			"DefaultShell.cfg"
			);
		Directory.CreateDirectory(Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
			".sudo"));
		File.WriteAllText(configFile, shellName);
	}

	public static void RunAsComlet(string comlet, string[]? args = null)
	{
		// 读取当前 shell 路径
		string shellPath = GetUserDefaultShell().Trim();
		string arguments = "";

		// 拼接剩余参数
		string argString = args != null && args.Length > 0
			? string.Join(" ", args.Select(a => $"\"{a}\""))
			: "";

		if (shellPath.EndsWith("pwsh.exe", StringComparison.OrdinalIgnoreCase) ||
			shellPath.EndsWith("powershell.exe", StringComparison.OrdinalIgnoreCase))
		{
			// PowerShell: -NoProfile -Command "<comlet> <args>"
			arguments = $"-NoProfile -Command \"{comlet} {argString}\"";
		}
		else if (shellPath.EndsWith("cmd.exe", StringComparison.OrdinalIgnoreCase))
		{
			// cmd: /c <comlet> <args>
			arguments = $"/c {comlet} {argString}";
		}
		else
		{
			// 其他 shell，直接传递命令和参数
			arguments = $"{comlet} {argString}".Trim();
		}

		var psi = new ProcessStartInfo
		{
			FileName = shellPath,

			UseShellExecute = false,
			WorkingDirectory = Directory.GetCurrentDirectory(),
			Verb = "runas" // UAC 提权
		};

		try
		{
			Process.Start(psi);
		}
		catch (Win32Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;	
			Console.Error.WriteLine($"启动失败: {ex.Message}");
			Console.ResetColor();
		}
	}
}