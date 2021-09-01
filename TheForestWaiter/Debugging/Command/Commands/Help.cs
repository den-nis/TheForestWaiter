using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("help", "Shows commands")]
    class Help : ICommand
    {
        public void Execute(object sender, string[] args)
        {
            if (sender is CommandHandler handler)
            {
                foreach (var m in handler.CommandInfo.Values)
                {
                    Console.Write($"\t- {ClipStr(m.Attribute.Name, 15, ' ')}");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"|{ClipStr(m.Attribute.Description, 40, ' ')}|");
                    Console.ResetColor();
                }
                Console.WriteLine("use \"usage {command}\" to get info about parameters");
            }
        }

        private static string ClipStr(string value, int length, char pad)
        {
            value ??= string.Empty;

            if (value.Length > length)
                return value.Substring(0, length);

            if (value.Length < length)
                return value + new string(pad, length - value.Length);

            return value;
        }
    }
}
