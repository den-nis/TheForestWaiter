using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using TheForestWaiter.States;

namespace TheForestWaiter.Debugging
{
    public static partial class Commands
    {
        [Command("help", "Shows commands")]
        public static void Help()
        {
            var commands = GameDebug.GetAllCommandInfos();

            foreach (var m in commands)
            {
                Console.Write($"\t- {ClipStr(m.Attribute.Name, 15, ' ')}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"|{ClipStr(m.Attribute.Description, 40, ' ')}|");
                Console.ResetColor();
            }
		    Console.WriteLine("use \"usage {command}\" to get info about parameters");
        }

        [Command("usage", "Shows how to use a command", "usage {command}")]
        public static void Usage(string[] args)
        {
            var commands = GameDebug.GetCommandInfo(args[0]);
			Console.WriteLine(commands.Attribute.Usage ?? "command has no parameters");
        }

        [Command("clear", "Clears the console")]
        public static void Clear() => Console.Clear();

        [Command("log", null, "Shows the log")]
        public static void Log()
        {
            foreach(var log in GameDebug.Logs)
            {
                Console.WriteLine(log);
            }
        }

        [Command("set", "Set a variable", "set {name} {value}")]
        public static void SetVar(string[] args)
        {
            if (GameDebug.Variables.ContainsKey(args[0]))
            {
                GameDebug.Variables[args[0]] = Convert.ChangeType(args[1], GameDebug.Variables[args[0]].GetType());
            }
            else
            {
                Console.WriteLine($"Unknown var \"{args[0]}\"");
            }
        }

        [Command("getvars", "Get current variables")]
        public static void GetVars()
        {
            foreach(var i in GameDebug.Variables)
            {
                Console.WriteLine($"\t- {i.Key} = {i.Value} : {i.Value.GetType().Name}");
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
