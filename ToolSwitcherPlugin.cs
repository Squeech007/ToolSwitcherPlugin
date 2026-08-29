using Sandbox.Game.World;
using System;
using VRage.Plugins;
using VRage.Utils;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.Game.Components;
using Sandbox.ModAPI;
using Sandbox.Game.Entities.Character;
using avaness.ToolSwitcherPlugin.Tools;
using Sandbox.Common.ObjectBuilders.Definitions;
using avaness.ToolSwitcherPlugin.Slot;
using VRage.Input;
using DarkHelmet.BuildVision2;
using avaness.ToolSwitcherPlugin.Definitions;

namespace avaness.ToolSwitcherPlugin
{
    public class ToolSwitcherPlugin : IPlugin, IDisposable
    {
        [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
        public class ToolPluginSession : MySessionComponentBase
        {
            public static ToolPluginSession Instance;

            public ToolDefinitions Definitions { get; } = new ToolDefinitions();

            // --- Nexus seamless-switch fix ---
            // Space Engineers normally recreates this whole session component
            // (Init -> ... -> UnloadData) whenever you connect to a different
            // server, so the one-shot "start" flag below was always safe.
            // Nexus's SeamlessClient plugin deliberately skips that teardown
            // when hopping between servers in the same cluster to avoid a
            // loading screen, so this component (and MySession.Static.LocalHumanPlayer)
            // can silently become stale after a hop.
            //
            // NOTE: SeamlessClient does have a SeamlessServerLoaded()/
            // SeamlessServerUnloaded() reflection hook, but its own log
            // ("Mod Assembly: X.Session has SeamlessServerLoaded/Unloaded
            // methods!") shows it only scans Workshop *mod* session
            // components (MySession.Static.Mods), not Pulsar/PluginLoader
            // *plugins* - which are injected before a session even exists
            // and never appear in that mod list. So this plugin can never
            // be discovered that way, regardless of method naming - it's a
            // mod-vs-plugin gap in SeamlessClient, not a naming bug here.
            //
            // The fix is a per-tick reference check instead: it's a single
            // pointer comparison with no allocation and no network/server
            // involvement, so running it every tick (rather than on a
            // timer) is effectively free and removes any detection delay.
            private MyPlayer cachedPlayer;

            private void Rebind(MyPlayer player)
            {
                if (inv != null)
                {
                    inv.Unload();
                }

                inv = new PlayerCharacter(player, Definitions);
                cachedPlayer = player;
            }
            // --- end Nexus seamless-switch fix ---

            private bool start;
            private ToolGroup group;
            private PlayerCharacter inv;

            public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
            {
                Instance = this;
                new BvApiClient();
                BvApiClient.Init("ToolSwitcherPlugin");
                MyLog.Default.WriteLineAndConsole("Tool Plugin Session loaded.");
            }

            protected override void UnloadData()
            {
                if (inv != null)
                {
                    inv.Unload();
                }
                Instance = null;
            }

            public override void UpdateAfterSimulation()
            {
                if (MySession.Static == null)
                    return;

                MyCharacter ch = MySession.Static.LocalCharacter;
                if (ch == null)
                    return;

                if (!start)
                    Start();

                // Cheap per-tick check: a Nexus seamless switch swaps out
                // MySession.Static.LocalHumanPlayer without going through
                // UnloadData()/Init(), so "start" alone won't catch it.
                // This is just a reference comparison - no alloc, no cost
                // worth throttling.
                MyPlayer currentPlayer = MySession.Static.LocalHumanPlayer;
                if (currentPlayer != null && currentPlayer != cachedPlayer)
                {
                    MyLog.Default.WriteLineAndConsole("Tool Plugin: local player changed without a session reload (Nexus seamless switch) - rebinding.");
                    Rebind(currentPlayer);
                }

                int input = MyInput.Static.DeltaMouseScrollWheelValue();

                if (ch.ToolbarType == MyToolbarType.Character && inv.Toolbar != null && inv.Inventory != null && IsEnabled())
                {
                    if (inv.Toolbar.NeedsTool)
                    {
                        inv.CheckForUpgrade = false;
                        group.EquipAny(inv);
                    }
                    else if (inv.CheckForUpgrade)
                    {
                        inv.CheckForUpgrade = false;
                        group.EquipUpgrade(inv);
                    }

                    if (input != 0)
                    {
                        group.EquipNext(inv, input > 0);
                    }
                }
            }

            private void Start()
            {
                Definitions.Add<MyObjectBuilder_WelderDefinition>();
                Definitions.Add<MyObjectBuilder_AngleGrinderDefinition>();
                Definitions.Add<MyObjectBuilder_HandDrillDefinition>();
                group = new ToolGroup(Definitions);
                Rebind(MySession.Static.LocalHumanPlayer);
                DisableModVersion();
                start = true;
            }

            private bool IsEnabled()
            {
                return MyAPIGateway.Gui.GetCurrentScreen == MyTerminalPageEnum.None && !MyAPIGateway.Gui.IsCursorVisible && !MyAPIGateway.Gui.ChatEntryVisible
                    && !MyAPIGateway.Session.IsCameraUserControlledSpectator && string.IsNullOrWhiteSpace(MyAPIGateway.Gui.ActiveGamePlayScreen) && (!BvApiClient.Registered || !BvApiClient.Open);
            }

            private class ItemEvent
            {
                public HandItem Item { get; }
                public ToolSlot Slot { get; }

                public ItemEvent(HandItem item, ToolSlot slot)
                {
                    Item = item;
                    Slot = slot;
                }
            }

            private void DisableModVersion()
            {
                MyAPIGateway.Utilities.SendModMessage(2211605465, false);
            }
        }

        public void Dispose()
        {
        }

        public void Init(object gameInstance)
        {
        }

        public void Update()
        {
        }
    }
}
