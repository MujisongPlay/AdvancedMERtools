using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Enums;

namespace AdvancedMERTools
{
    public class HealthObjectDeadEventArgs : EventArgs, Exiled.Events.EventArgs.Interfaces.IExiledEvent
    {
        public HealthObjectDeadEventArgs(HealthObjectDTO healthObject, Player Attacker)
        {
            HealthObject = healthObject;
            Killer = Attacker;
        }

        public HealthObjectDTO HealthObject;
        public Player Killer;
    }
}
