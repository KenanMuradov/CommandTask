using Models.Classes;
using Models.Enums;
using Server;
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


    new Task(() =>
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
                        var helpText = ExecuteServerCommands.HelpText();
                        bw.Write(helpText);
                        stream.Flush();
                        break;
                    }
                case CommandTexts.Proclist:
                    {
                        var jsonList=ExecuteServerCommands.GetProcessListJson();
                        bw.Write(jsonList);
                        stream.Flush();
                        break;
                    }
                case CommandTexts.Kill:
                    {
                        var canKill = ExecuteServerCommands.KillProcess(command.Parameter);
                        bw.Write(canKill);
                        break;
                    }
                case CommandTexts.Run:
                    {
                        var canRun = ExecuteServerCommands.RunProcess(command.Parameter);
                        bw.Write(canRun);
                        break;
                    }
                case CommandTexts.Unkown:
                    break;
            }
        }
    }).Start();
}

