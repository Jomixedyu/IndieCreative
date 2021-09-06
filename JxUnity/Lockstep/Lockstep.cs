using System;
using System.Collections.Generic;
using UnityEngine;

namespace JxUnity.Lockstep
{
    [Serializable]
    public class LockstepCommand
    {

    }

    [Serializable]
    public class LockstepCommandMove : LockstepCommand
    {
        public float horizontal;
        public float vertical;

        public LockstepCommandMove()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }
    }

    public delegate void LockstepUpdate(int frame);

    public class Lockstep
    {
        public bool IsRecord { get; set; }
        public bool IsEnabled { get; set; }
        public event LockstepUpdate OnUpdate;

        LinkedList<List<LockstepCommand>> cmds = null;
        
        int frame = -1;

        Dictionary<string, Action<LockstepCommand>> msgcb;

        public Lockstep()
        {
            this.cmds = new LinkedList<List<LockstepCommand>>();
            this.msgcb = new Dictionary<string, Action<LockstepCommand>>();
        }
        public void Tick()
        {
            if(!this.IsEnabled)
            {
                return;
            }
            ++this.frame;
            this.cmds.AddLast(new List<LockstepCommand>());
            this.OnUpdate?.Invoke(this.frame);
        }
        public void SendMessage(LockstepCommand command)
        {
            this.cmds.Last.Value.Add(command);
            if (this.msgcb.TryGetValue(command.GetType().Name, out var v))
            {
                v?.Invoke(command);
            }
        }
        public void Register(string msg, Action<LockstepCommand> cmd)
        {
            this.msgcb.Add(msg, cmd);
        }
    }
}
