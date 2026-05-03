using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

using Build.Models;

namespace Build.Services;

public static class ProcessService
{
  public static Process StartComponent(RuntimeComponent component)
  {
    var startInfo = new ProcessStartInfo
    {
      FileName = component.FileName,
      WorkingDirectory = component.WorkingDirectory,
      UseShellExecute = true,
    };

    foreach (var argument in component.Arguments)
    {
      startInfo.ArgumentList.Add(argument);
    }

    return Process.Start(startInfo)
        ?? throw new InvalidOperationException($"Failed to start process '{component.Name}'.");
  }

  public static bool WaitForTcpPort(int port, TimeSpan timeout)
  {
    var deadline = DateTime.UtcNow + timeout;
    while (DateTime.UtcNow < deadline)
    {
      var connected = false;

      try
      {
        using var client = new TcpClient();
        var connectTask = client.ConnectAsync("127.0.0.1", port);
        connected = connectTask.Wait(TimeSpan.FromMilliseconds(250)) && client.Connected;
      }
      catch (Exception)
      {
        connected = false;
      }

      if (connected)
      {
        return true;
      }

      Thread.Sleep(300);
    }

    return false;
  }

  public static bool IsProcessRunning(int pid)
  {
    try
    {
      var process = Process.GetProcessById(pid);
      return !process.HasExited;
    }
    catch
    {
      return false;
    }
  }

  public static void TryStop(int pid)
  {
    try
    {
      var process = Process.GetProcessById(pid);
      if (process.HasExited)
      {
        return;
      }

      if (process.MainWindowHandle != IntPtr.Zero)
      {
        process.CloseMainWindow();
        if (process.WaitForExit(5000))
        {
          return;
        }
      }

      process.Kill(true);
    }
    catch (Exception)
    {
      return;
    }
  }
}
