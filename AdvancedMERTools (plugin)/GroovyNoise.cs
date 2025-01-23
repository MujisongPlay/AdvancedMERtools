using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Items;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features;
using Exiled.CustomItems;
using CommandSystem;
using Utf8Json.Formatters;
using RemoteAdmin;
using Exiled.CustomItems.API.Features;

namespace AdvancedMERTools
{
    public class GroovyNoise : AMERTInteractable
    {
        protected virtual void Start()
        {
            Base = base.Base as GNDTO;
            AdvancedMERTools.Singleton.groovyNoises.Add(this);
            //MEC.Timing.CallDelayed(0.1f, () => 
            //{
            //    if (AdvancedMERTools.Singleton.groovyNoises.All(x => x.Base.GMDTOs.Select(y => y.codes).All(y => !y.Contains(Base.Code))))
            //        Active = true;
            //});
        }

        protected virtual void Update()
        {
            if (Active)
            {
                //ServerConsole.AddLog("!!!");
                GMDTO.Execute(Base.Settings, new ModuleGeneralArguments { schematic = OSchematic, transform = transform });
            }
            Active = false;
        }

        public new GNDTO Base;
    }

    public class FGroovyNoise : GroovyNoise
    {
        protected override void Start()
        {
            Base = ((AMERTInteractable)this).Base as FGNDTO;
            AdvancedMERTools.Singleton.groovyNoises.Add(this);
            //MEC.Timing.CallDelayed(0.1f, () => 
            //{
            //    if (AdvancedMERTools.Singleton.groovyNoises.All(x => x.Base.GMDTOs.Select(y => y.codes).All(y => !y.Contains(Base.Code))))
            //        Active = true;
            //});
        }

        protected override void Update()
        {
            if (Active)
            {
                //ServerConsole.AddLog("!!!");
                FGMDTO.Execute(Base.Settings, new FunctionArgument(this));
            }
            Active = false;
        }

        public new FGNDTO Base;
    }
}
