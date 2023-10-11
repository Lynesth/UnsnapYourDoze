using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using System;

namespace UnsnapYourDoze
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Unsnap Your Doze";
        private const string CommandName = "/dozesnap";

        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        private Configuration Configuration { get; init; }
        [PluginService] private static IChatGui Chat { get; set; } = null!;
        [PluginService] private static IGameInteropProvider GameInteropProvider { get; set; } = null!;

        private delegate byte ShouldSnapDelegate(IntPtr a1, IntPtr a2);
        [Signature("E8 ?? ?? ?? ?? 84 C0 74 44 4C 8D 6D C7", DetourName = nameof(ShouldSnapDetour))]
        private readonly Hook<ShouldSnapDelegate>? ShouldSnapHook = null!;

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            GameInteropProvider.InitializeFromAttributes(this);
            this.ShouldSnapHook.Enable();

            this.CommandManager.AddHandler(CommandName, new CommandInfo(ToggleSnap)
                { HelpMessage = "Toggles doze snap on/off." });
        }

        public void Dispose()
        {
            this.CommandManager.RemoveHandler(CommandName);
            this.ShouldSnapHook?.Dispose();
        }

        private byte ShouldSnapDetour(IntPtr a1, IntPtr a2)
        {
            return this.Configuration.DisableSnap
                ? (byte) 0
                : this.ShouldSnapHook!.Original(a1, a2);
        }

        private void ToggleSnap(string command, string arguments)
        {
            this.Configuration.DisableSnap ^= true;
            this.Configuration.Save();

            var un = this.Configuration.DisableSnap ? "un" : "";
            Chat.Print($"/doze is now {un}snapped.");
        }
    }
}
