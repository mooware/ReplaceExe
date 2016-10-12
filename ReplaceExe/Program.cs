using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ReplaceExe
{
  class Program
  {
    const string regBasePath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options";
    const string regValueName = "Debugger";

    static void Main(string[] args)
    {
      try
      {
        if (args.Length > 2 && args[0] == "-i") // -i <orig exe> <command args...>
          Install(args[1], args.Skip(2).ToArray());
        else if (args.Length > 1 && args[0] == "-u") // -u <orig exe>
          Uninstall(args[1]);
        else if (args.Length > 2) // <args> <command args...> <orig exe> file1 file2 ...
          Run(args);
        else
          PrintHelp();
      }
      catch (Exception ex)
      {
        System.Console.Error.WriteLine(string.Join(" ", args));
        // just catch and print anything
        System.Console.Error.WriteLine(ex.ToString());
      }
    }

    static void Install(string image, string[] command)
    {
      string myPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
      // <this exe> <number of following args> <args...>
      string fullCommand = string.Format("\"{0}\" {1} {2}", myPath, command.Length, string.Join(" ", command));

      var baseKey = Registry.LocalMachine.OpenSubKey(regBasePath, true);
      var key = baseKey.CreateSubKey(image);
      key.SetValue(regValueName, fullCommand);
    }

    static void Uninstall(string image)
    {
      var baseKey = Registry.LocalMachine.OpenSubKey(regBasePath);
      var key = baseKey.OpenSubKey(image);
      if (key != null)
        key.DeleteValue(regValueName, false);
    }

    static void Run(string[] args)
    {
      int len = int.Parse(args[0]);
      string command = args[1];
      string[] commandArgs = args.Skip(2).Take(len - 1).ToArray();
      string[] replaceArgs = args.Skip(2 + len).Select(a => '"' + a + '"').ToArray();
      string joinedArgs = string.Join(" ", commandArgs) + ' ' + string.Join(" ", replaceArgs);

      Process proc = Process.Start(command, joinedArgs);
      // stop when the other process stops, to allow waiting on the replaced exe
      proc.WaitForExit();
    }

    static void PrintHelp()
    {
      System.Console.WriteLine("usage:");
      System.Console.WriteLine();
      System.Console.WriteLine("install:");
      System.Console.WriteLine("  -i <image to replace> <command> <args...>");
      System.Console.WriteLine("uninstall:");
      System.Console.WriteLine("  -u <image to replace>");
    }
  }
}
