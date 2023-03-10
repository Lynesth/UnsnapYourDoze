using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace UnsnapYourDoze
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public bool DisableSnap { get; set; } = true;
        [NonSerialized] private DalamudPluginInterface? PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface) => this.PluginInterface = pluginInterface;

        public void Save() => this.PluginInterface!.SavePluginConfig(this);
    }
}
