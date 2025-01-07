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
        void Start()
        {
            Base = base.Base as GNDTO;
            AdvancedMERTools.Singleton.groovyNoises.Add(this);
            AdvancedMERTools.Singleton.codeClassPair.Add(Base.Code, this);
            //MEC.Timing.CallDelayed(0.1f, () => 
            //{
            //    if (AdvancedMERTools.Singleton.groovyNoises.All(x => x.Base.GMDTOs.Select(y => y.codes).All(y => !y.Contains(Base.Code))))
            //        Active = true;
            //});
        }

        void Update()
        {
            if (Active)
            {
                GMDTO.GetSingleton<GMDTO>().Execute(GMDTO.SelectList<GMDTO>(Base.Settings));
            }
            Active = false;
        }

        public new GNDTO Base;
    }
}
