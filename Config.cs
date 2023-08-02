using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Loader;
using Exiled.API.Interfaces;

namespace MERDummyDoorInstaller
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public List<string> DummyDoorInstallingMaps { get; set; } = new List<string>
        {
            "ExampleMap"
        };
        [Description("Below index means the door numbering that do not require dummy door installing. Caution: It's double List. - (Above Map Index) - (Door Index)")]
        public List<List<int>> BlackListOfDummyDoor { get; set; } = new List<List<int>>
        {
            new List<int>
            {
                101
            },
            new List<int>
            {
                15
            }
        };
    }
}
