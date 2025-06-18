using System.Diagnostics;
using Microsoft.Win32;

namespace WindowsSudo
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("错误: 请提供要执行的命令。");
				Console.WriteLine("用法: sudo [命令] [参数...]");
				Environment.Exit(1);
			}

			try
			{
				bool isUacEnabled = IsUacEnabled();
				string fullCommandLine = string.Join(" ", args);

				ExecuteWithAdminPrivileges(args[0], fullCommandLine);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"执行过程中发生错误: {ex.Message}");
				Environment.Exit(1);
			}
		}

		static bool IsUacEnabled()
		{
			try
			{
				using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");
				if (key == null)
					return false;

				var value = key.GetValue("EnableLUA");
				return value != null && (int)value == 1;
			}
			catch
			{
				return true;
			}
		}

		static void ExecuteWithAdminPrivileges(string command, string fullCommandLine)
		{
			bool isPowerShellCommand = IsPowerShellCommand(command) || IsPowerShellVerbNounCommand(command);
			bool isCmdInternalCommand = IsCmdInternalCommand(command);

			ProcessStartInfo startInfo = new()
			{
				UseShellExecute = true,
				Verb = "runas"
			};

			if (isPowerShellCommand)
			{
				startInfo.FileName = "powershell.exe";
				startInfo.Arguments = $"-NoProfile -Command \"{fullCommandLine}\"";
			}
			else if (isCmdInternalCommand)
			{
				startInfo.FileName = "cmd.exe";
				startInfo.Arguments = $"/c {fullCommandLine}";
			}
			else
			{
				startInfo.FileName = command;
				if (fullCommandLine.Length > command.Length)
				{
					startInfo.Arguments = fullCommandLine[command.Length..].TrimStart();
				}
			}

			try
			{
				Process.Start(startInfo);
			}
			catch (System.ComponentModel.Win32Exception ex)
			{
				if (ex.NativeErrorCode == 1223)
				{
					Console.WriteLine("操作已被用户取消。");
					Environment.Exit(1);
				}
				throw;
			}
		}

		static bool IsPowerShellCommand(string command)
		{
			return command.Equals("powershell", StringComparison.OrdinalIgnoreCase) ||
				   command.Equals("pwsh", StringComparison.OrdinalIgnoreCase);
		}

		static bool IsPowerShellVerbNounCommand(string command)
		{


			if (!command.Contains('-'))
				return false;

			string[] parts = command.Split('-', 2);
			if (parts.Length != 2)
				return false;
			return Array.Exists(GetPowerShellApprovedVerbs(), v => v.Equals(parts[0], StringComparison.OrdinalIgnoreCase)) &&
				   !string.IsNullOrEmpty(parts[1]);
		}
		static string[] GetPowerShellApprovedVerbs()
		{
			try
			{
				// 使用 PowerShell 获取所有可用的命令的动词部分
				var psi = new ProcessStartInfo
				{
					FileName = "powershell.exe",
					Arguments = "-NoProfile -Command \"Get-Command -CommandType Cmdlet,Function | Select-Object -ExpandProperty Verb | Sort-Object -Unique\"",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				using var process = Process.Start(psi);
				var output = process?.StandardOutput.ReadToEnd();
				process?.WaitForExit();
				var verbs = output?.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
				return verbs ?? [];
				;
			}
			catch
			{
				// 失败时回退到常用动词
				return [
						"Add", "Clear", "Close", "Copy", "Enter", "Exit", "Find", "Format", "Get",
						"Hide", "Join", "Lock", "Move", "New", "Open", "Optimize", "Pop", "Push",
						"Remove", "Rename", "Reset", "Search", "Select", "Set", "Show", "Skip",
						"Split", "Stop", "Submit", "Suspend", "Switch", "Test", "Trace", "Unlock", "Watch"
					];
			}
		}
		static bool IsCmdInternalCommand(string command)
		{
			string[] internalCommands = [
				"cd", "dir", "type", "echo", "mkdir", "rmdir", "md", "rd",
				"copy", "move", "del", "erase", "ren", "cls", "ver", "help", "exit"
			];

			return Array.Exists(internalCommands,
				cmd => cmd.Equals(command, StringComparison.OrdinalIgnoreCase));
		}
	}
}