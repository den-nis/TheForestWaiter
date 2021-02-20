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
            var commands = GameDebug.GetAllCommandInfo();

            foreach (var m in commands)
            {
                if (string.IsNullOrWhiteSpace(m.Attribute.Usage))
                    Console.WriteLine($"\t- {m.Attribute.Name}");
                else
                    Console.WriteLine($"\t- {m.Attribute.Name} : {m.Attribute.Usage}");
            }
        }

        [Command("info", "shows more info about command", "info {command}")]
        public static void Info(string[] args)
        {
            var command = GameDebug.GetCommandInfo(args[0]);

            if (!string.IsNullOrWhiteSpace(command.Attribute.Usage))
                Console.WriteLine($"\tUsage       : {command.Attribute.Usage}");

            if (!string.IsNullOrWhiteSpace(command.Attribute.Description))
                Console.WriteLine($"\tDescription : {command.Attribute.Description}");
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

        [Command("nl", "no logging")]
        public static void NoLogging()
        {
            GameDebug.Variables["output_log"] = false;
			Console.WriteLine("turned off logging");
        }
    }
}
