using Models.Classes;
using Models.Enums;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 45678);

listener.Start(10);



while (true)
{
    var client = await listener.AcceptTcpClientAsync();

    Console.WriteLine($"Client {client.Client.RemoteEndPoint} accepted");

    Task.Run(() =>
    {
        var stream = client.GetStream();
        var bw = new BinaryWriter(stream);
        var br = new BinaryReader(stream);
        while (true)
        {
            var jsonStr = br.ReadString();

            var command = JsonSerializer.Deserialize<Command>(jsonStr);

            if (command is null)
                return;

            switch (command.Text)
            {
                case CommandTexts.Help:
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append("\nproclist".PadRight(40));
                        builder.Append("see all processes");
                        builder.Append("\nkill <process name>".PadRight(40));
                        builder.Append("end the given process");
                        builder.Append("\nrun <process name>".PadRight(40));
                        builder.Append("run the given process");
                        bw.Write(builder.ToString());
                        stream.Flush();
                        break;
                    }
                case CommandTexts.Proclist:
                    {
                        var list = Process.GetProcesses()
                        .Select(p => p.ProcessName)
                        .ToList();
                        var jsonList = JsonSerializer.Serialize(list);
                        bw.Write(jsonList);
                        stream.Flush();
                        break;
                    }
                case CommandTexts.Kill:
                    {
                        var canKill = false;
                        var processes = Process.GetProcessesByName(command.Parameter);

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

                        bw.Write(canKill);
                        break;
                    }
                case CommandTexts.Run:
                    {
                        var canRun = false;

                        if(command.Parameter is not null)
                        {
                            try
                            {
                                Process.Start(command.Parameter);
                                canRun = true;
                            }
                            catch (Exception) { }
                        }
                        bw.Write(canRun);
                        break;
                    }
                case CommandTexts.Unkown:
                    break;
                default:
                    break;
            }
        }
    });
}

