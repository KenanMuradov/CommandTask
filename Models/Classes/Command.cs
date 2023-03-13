using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Classes;

public class Command
{
    public CommandTexts Text { get; set; }
    public string? Parameter { get; set; }
}
