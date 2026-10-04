#!/bin/bash
set -e

SEBIN="$HOME/.local/share/Steam/steamapps/common/SpaceEngineers/Bin64"

echo "Building ToolSwitcherPlugin for Pulsar Interim (.NET 10)..."

dotnet new classlib \
  --name ToolSwitcherInterimBuild \
  --framework net10.0 \
  --force \
  --no-restore >/dev/null

cd ToolSwitcherInterimBuild

cat > ToolSwitcherInterimBuild.csproj <<EOF2
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>disable</ImplicitUsings>
    <Nullable>disable</Nullable>
    <LangVersion>latest</LangVersion>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <AssemblyName>ToolSwitcherPlugin</AssemblyName>
    <RootNamespace>avaness.ToolSwitcherPlugin</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="ProtoBuf.Net">
      <HintPath>$SEBIN/ProtoBuf.Net.dll</HintPath>
    </Reference>
    <Reference Include="Sandbox.Common">
      <HintPath>$SEBIN/Sandbox.Common.dll</HintPath>
    </Reference>
    <Reference Include="Sandbox.Game">
      <HintPath>$SEBIN/Sandbox.Game.dll</HintPath>
    </Reference>
    <Reference Include="Sandbox.Graphics">
      <HintPath>$SEBIN/Sandbox.Graphics.dll</HintPath>
    </Reference>
    <Reference Include="SpaceEngineers.Game">
      <HintPath>$SEBIN/SpaceEngineers.Game.dll</HintPath>
    </Reference>
    <Reference Include="SpaceEngineers.ObjectBuilders">
      <HintPath>$SEBIN/SpaceEngineers.ObjectBuilders.dll</HintPath>
    </Reference>
    <Reference Include="VRage">
      <HintPath>$SEBIN/VRage.dll</HintPath>
    </Reference>
    <Reference Include="VRage.Game">
      <HintPath>$SEBIN/VRage.Game.dll</HintPath>
    </Reference>
    <Reference Include="VRage.Input">
      <HintPath>$SEBIN/VRage.Input.dll</HintPath>
    </Reference>
    <Reference Include="VRage.Library">
      <HintPath>$SEBIN/VRage.Library.dll</HintPath>
    </Reference>
    <Reference Include="VRage.Math">
      <HintPath>$SEBIN/VRage.Math.dll</HintPath>
    </Reference>
    <Reference Include="VRage.Network">
      <HintPath>$SEBIN/VRage.Network.dll</HintPath>
    </Reference>
    <Reference Include="VRage.Render">
      <HintPath>$SEBIN/VRage.Render.dll</HintPath>
    </Reference>
    <Reference Include="VRage.Render11">
      <HintPath>$SEBIN/VRage.Render11.dll</HintPath>
    </Reference>
    <Reference Include="VRage.Scripting">
      <HintPath>$SEBIN/VRage.Scripting.dll</HintPath>
    </Reference>
    <Reference Include="VRage.UserInterface">
      <HintPath>$SEBIN/VRage.UserInterface.dll</HintPath>
    </Reference>
  </ItemGroup>

</Project>
EOF2

rm -f Class1.cs

cp ../BvAPIClient.cs .
cp ../PlayerCharacter.cs .
cp ../PlayerToolbar.cs .
cp ../ToolSwitcherPlugin.cs .
cp ../Properties/AssemblyInfo.cs .

cp -r ../Definitions .
cp -r ../Slot .
cp -r ../Tools .

dotnet build -c Release

cp -f bin/Release/net10.0/ToolSwitcherPlugin.dll ../ToolSwitcherPlugin.Interim.dll

echo
echo "BUILD SUCCESS: ToolSwitcherPlugin.Interim.dll"
