using System;
using AltV.Net.Elements.Entities;

namespace AltV.Net.Elements.Refs
{
    public readonly struct BlipRef : IDisposable
    {
        private readonly IBlip blip;

        public bool Exists => blip != null;

        public BlipRef(IBlip blip)
        {
            this.blip = blip.AddRef() ? blip : null;
            Alt.Module.CountUpRefForCurrentThread(blip);
        }

        public void Dispose()
        {
            blip?.RemoveRef();
            Alt.Module.CountDownRefForCurrentThread(blip);
        }
    }
}