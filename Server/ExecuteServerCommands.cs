using Models.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server;

public static class ExecuteServerCommands
{
    public static string HelpText()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("\nproclist".PadRight(40));
        builder.Append("see all processes");
        builder.Append("\nkill <process name>".PadRight(40));
        builder.Append("end the given process");
        builder.Append("\nrun <process name>".PadRight(40));
        builder.Append("run the given process");

        return builder.ToString();
    }

    public static string GetProcessListJson()
    {
        var list = Process.GetProcesses()
                        .Select(p => p.ProcessName)
                        .ToList();
        var jsonList = JsonSerializer.Serialize(list);

        return jsonList;
    }

    public static bool KillProcess(string? processName)
    {
        if(processName == null) return false;

        var canKill = false;
        var processes = Process.GetProcessesByName(processName);

        if (processes.Length > 0)
        {
            try
            {
                foreach (var p in processes)
                    p.Kill();

                canKill = true;
            }
            catch (Exception) { }
        }


        return canKill;
    }

    public static bool RunProcess(string? processName)
    {
        var canRun = false;

        if (processName is not null)
        {
            try
            {
                Process.Start(processName);
                canRun = true;
            }
            catch (Exception) { }
        }

        return canRun;
    }
}
