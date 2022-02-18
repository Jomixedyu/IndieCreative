using System;
using System.Collections.Generic;
using UnityEngine;

namespace JxUnity.Utility
{
    public class PlayerConsoleFunction
    {
        public string name;
        public string description;
        public Delegate function;
        public string help;
    }

    public static class PlayerConsole
    {
        private static Dictionary<string, PlayerConsoleFunction> cmds;

        public static event Action<string> MessageChannel;

        public static void Register(PlayerConsoleFunction func)
        {
            cmds.Add(func.name, func);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void UnityStaticConstructor()
        {
            cmds = new Dictionary<string, PlayerConsoleFunction>();

            Register(new PlayerConsoleFunction()
            {
                name = "?",
                function = Help,
                description = "all command",
                help = "/?",
            });
            Register(new PlayerConsoleFunction()
            {
                name = "help",
                function = HelpFile,
                description = "help file",
                help = "/help command"
            });
        }

        private static void Help()
        {
            foreach (var item in cmds)
            {
                Print($"/{item.Key} {item.Value.description}");
            }
        }

        private static void HelpFile(string name)
        {
            Print(cmds[name].help);
        }

        private static List<string> args = new List<string>(8);

        private static List<string> Parse(string cmd)
        {
            args.Clear();

            int pos = 1;
            int length = cmd.Length;
            int begin = 1;

            while (pos < length)
            {
                if (cmd[pos] == ' ')
                {
                    if (begin == pos)
                    {
                        begin++;
                        pos++;
                        continue;
                    }
                    args.Add(cmd.Substring(begin, pos - begin));
                    begin = pos + 1;
                }

                pos++;
            }
            //end
            if (begin != pos)
            {
                args.Add(cmd.Substring(begin, pos - begin));
            }

            return args;
        }

        /// <summary>
        /// 以/开头的指令
        /// </summary>
        public static bool Execute(string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd))
            {
                return false;
            }
            cmd = cmd.Trim();
            if (cmd[0] != '/')
            {
                Print(cmd);
                Print("error: FormatExpcetion");
                return false;
            }

            var args = Parse(cmd);

            if (args.Count == 0)
            {
                return false;
            }
            int paramCount = args.Count - 1;

            Print(cmd); //echo

            PlayerConsoleFunction func = null;

            if (!cmds.TryGetValue(args[0], out func))
            {
                Print("error: function not found");
                return false;
            }

            var deleg = func.function;
            var methodInfo = deleg.Method;

            var paramsInfo = methodInfo.GetParameters();

            //params error | name only
            if (paramCount != paramsInfo.Length || args.Count == 1)
            {
                //help
                Print(func.help);
                return false;
            }

            object[] objParams = new object[paramsInfo.Length];
            try
            {
                for (int i = 0; i < paramCount; i++)
                {
                    objParams[i] = Convert.ChangeType(args[i + 1], paramsInfo[i].ParameterType);
                }
            }
            catch (Exception e)
            {
                Print(func.help);
                return false;
            }

            try
            {
                deleg.DynamicInvoke(objParams);
            }
            catch (Exception e)
            {
                Print("error: " + e.Message);
                return false;
            }

            return true;
        }

        public static void Print(string content)
        {
            MessageChannel?.Invoke(content);
        }
    }
}
