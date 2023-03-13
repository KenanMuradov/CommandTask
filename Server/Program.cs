using Models.Classes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var listener = new TcpListener(IPAddress.Parse("127.0.0.1"),45678);

listener.Start(10);



while (true)
{
    var client = await listener.AcceptTcpClientAsync();

    Console.WriteLine($"Client {client.Client.RemoteEndPoint} accepted");

    await Task.Run(()=>
    {
        var stream = client.GetStream();
        var bw = new BinaryWriter(stream);
        var br = new BinaryReader(stream);

        var jsonStr= br.ReadString();

        var command = JsonSerializer.Deserialize<Command>(jsonStr);

        if (command is null)
            return;

        switch (command.Text)
        {
            case Models.Enums.CommandTexts.Help:
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
            case Models.Enums.CommandTexts.Proclist:
                break;
            case Models.Enums.CommandTexts.Kill:
                break;
            case Models.Enums.CommandTexts.Run:
                break;
            case Models.Enums.CommandTexts.Unkown:
                break;
            default:
                break;
        }
    });
}

