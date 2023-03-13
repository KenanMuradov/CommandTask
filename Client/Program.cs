using Models.Classes;
using Models.Enums;
using System.Net.Sockets;
using System.Text.Json;

var client = new TcpClient("127.0.0.1",45678);

var stream = client.GetStream();

var bw = new BinaryWriter(stream);
var br = new BinaryReader(stream);

var userCommand = string.Empty;
while (true)
{
    userCommand = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userCommand))
    {
        Console.WriteLine("Please enter command. Press any key to continue...");
        Console.ReadKey();
        Console.Clear();
        continue;
    }

    var commandProperties = userCommand.Split(' ');

    if (commandProperties.Length > 2)
    {
        Console.WriteLine("Given command does not match any template. Press any key to continue...");
        Console.ReadKey();
        Console.Clear();
        continue;
    }

    CommandTexts commandText = commandProperties[0].ToLower() switch
    {
        "help" => CommandTexts.Help,
        "proclist" => CommandTexts.Proclist,
        "kill" => CommandTexts.Kill,
        "run" => CommandTexts.Run,
        _ => CommandTexts.Unkown
    };

    string? commandParameter = default;
    if (commandProperties.Length == 2)
        commandParameter = commandProperties[1];


    var command = new Command()
    {
        Parameter = commandParameter,
        Text = commandText
    };


    switch (commandText)
    {
        case CommandTexts.Help:
            {
                if (!string.IsNullOrWhiteSpace(command.Parameter))
                {
                    Console.WriteLine("'help' command does not accept any parameter. Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }

                var jsonStr = JsonSerializer.Serialize(command);

                bw.Write(jsonStr);

                await Task.Delay(50);

                var response = br.ReadString();
                Console.WriteLine(response);
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
                break;
            }
        case CommandTexts.Proclist:
            {
                if (!string.IsNullOrWhiteSpace(command.Parameter))
                {
                    Console.WriteLine("'proclist' command does not accept any parameter. Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
                var jsonStr = JsonSerializer.Serialize(command);

                bw.Write(jsonStr);

                await Task.Delay(50);

                var response = br.ReadString();
                var list = JsonSerializer.Deserialize<List<string>>(response);

                if(list is null)
                {
                    Console.WriteLine("Something went wrong. Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
                Console.WriteLine();
                foreach (var processName in list)
                    Console.WriteLine(processName);

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
                break;
            }
        case CommandTexts.Kill:
            {

                if (string.IsNullOrWhiteSpace(command.Parameter))
                {
                    Console.WriteLine("You must declare <process name> to use 'kill' command. Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
                var jsonStr = JsonSerializer.Serialize(command);

                bw.Write(jsonStr);

                await Task.Delay(50);

                var response = br.ReadBoolean();
                if(response is true)
                {
                    Console.WriteLine("Process succesfully ended. Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("Process cannot be ended there is no active process by this name or access denied. \nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                break;
            }
        case CommandTexts.Run:
            break;
        case CommandTexts.Unkown:
            break;

    }

}