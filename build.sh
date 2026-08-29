#!/bin/bash
set -e

SEBIN="$HOME/.local/share/Steam/steamapps/common/SpaceEngineers/Bin64"

mcs \
  -target:library \
  -langversion:7.2 \
  -out:ToolSwitcherPlugin.dll \
  -reference:"$SEBIN/ProtoBuf.Net.dll" \
  -reference:"$SEBIN/Sandbox.Common.dll" \
  -reference:"$SEBIN/Sandbox.Game.dll" \
  -reference:"$SEBIN/Sandbox.Graphics.dll" \
  -reference:"$SEBIN/SpaceEngineers.Game.dll" \
  -reference:"$SEBIN/SpaceEngineers.ObjectBuilders.dll" \
  -reference:"$SEBIN/VRage.dll" \
  -reference:"$SEBIN/VRage.Game.dll" \
  -reference:"$SEBIN/VRage.Input.dll" \
  -reference:"$SEBIN/VRage.Library.dll" \
  -reference:"$SEBIN/VRage.Math.dll" \
  -reference:"$SEBIN/VRage.Network.dll" \
  -reference:"$SEBIN/VRage.Render.dll" \
  -reference:"$SEBIN/VRage.Render11.dll" \
  -reference:"$SEBIN/VRage.Scripting.dll" \
  -reference:"$SEBIN/VRage.UserInterface.dll" \
  -reference:"/usr/lib/mono/4.5/Facades/netstandard.dll" \
  BvAPIClient.cs Definitions/*.cs PlayerCharacter.cs PlayerToolbar.cs \
  Slot/*.cs Tools/*.cs ToolSwitcherPlugin.cs Properties/AssemblyInfo.cs

echo "BUILD SUCCESS: ToolSwitcherPlugin.dll"
