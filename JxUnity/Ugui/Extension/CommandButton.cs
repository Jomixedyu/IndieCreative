using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


namespace JxUnity.Ugui
{
    [Serializable]
    public class CommandButtonRecord
    {
        public string Command;
        public string Argument;
    }

    public class CommandButton : MonoBehaviour
    {
        [SerializeField]
        private List<CommandButtonRecord> commands;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void UnityStaticConstructor()
        {
            commandReg = new Dictionary<string, Action<string>>();
        }

        private static Dictionary<string, Action<string>> commandReg;

        public static void RegisterCommand(string Command, Action<string> executor)
        {
            commandReg.Add(Command, executor);
        }

        private void Awake()
        {
            if (commands == null)
            {
                commands = new List<CommandButtonRecord>();
            }

            this.GetComponent<Button>().onClick.AddListener(() =>
            {
                foreach (var item in commands)
                {
                    commandReg[item.Command]?.Invoke(item.Argument);
                }
            });

        }
    }
}