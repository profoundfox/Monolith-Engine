#!/usr/bin/env bash
set -e

read -p "Enter the project name: " PROJECT_NAME

PROJECT_DIR="./$PROJECT_NAME"

dotnet new mgdesktopgl -o "$PROJECT_DIR"

mkdir -p "$PROJECT_DIR/RawAssets"
mkdir -p "$PROJECT_DIR/Nodes"
mkdir -p "$PROJECT_DIR/Scenes"
mkdir -p "$PROJECT_DIR/Scripts"

mkdir -p "$PROJECT_DIR/RawContent"
mkdir -p "$PROJECT_DIR/RawContent/Textures"
mkdir -p "$PROJECT_DIR/RawContent/Audio"
mkdir -p "$PROJECT_DIR/RawContent/Raw"
mkdir -p "$PROJECT_DIR/RawContent/Font"

rm "$PROJECT_DIR/Game1.cs"

cat > "$PROJECT_DIR/Main.cs" << EOF
namespace $PROJECT_NAME
{
    public class Main : Engine
    {
        public Main() : base(new EngineConfig
        {
            Actions = new()
            {
                { "ExampleInput", new() { new InputAction(Keys.A), new InputAction(Buttons.A) } }
            }
        }) {}

        protected override void Initialize() => base.Initialize();
        protected override void Update(GameTime gameTime) => base.Update(gameTime);
        protected override void Draw(GameTime gameTime) => base.Draw(gameTime);
    }
}
EOF

cat > "$PROJECT_DIR/Nodes/ExampleNode.cs" << EOF
namespace $PROJECT_NAME 
{
    public record class ExampleNodeConfig : NodeConfig
    {
        public bool ExampleVariable { get; set; }
    }

    public class ExampleNode : Node
    {
        public ExampleNode(ExampleNodeConfig config) : base(config) {}

        public override void Load() => base.Load();
        public override void Unload() => base.Unload();
        public override void Update(GameTime gameTime) => base.Update(gameTime);
        public override void Draw(SpriteBatch spriteBatch) => base.Draw(spriteBatch);
    }
}
EOF

cat > "$PROJECT_DIR/Scenes/ExampleScene.cs" << EOF
namespace $PROJECT_NAME
{
    public class ExampleScene : Scene
    {
        public ExampleScene() : base(new SceneConfig { }) {}

        public override void Initialize() => base.Initialize();
        public override void Load() => base.Load();
        public override void Unload() => base.Unload();
        public override void Update(GameTime gameTime) => base.Update(gameTime);
        public override void Draw(SpriteBatch spriteBatch) => base.Draw(spriteBatch);
    }
}
EOF

cat > "$PROJECT_DIR/GlobalUsing.cs" << EOF
global using Monolith;
global using Monolith.Geometry;
global using Monolith.Graphics;
global using Monolith.Helpers;
global using Monolith.Input;
global using Monolith.IO;
global using Monolith.Managers;
global using Monolith.Nodes;
global using Monolith.UI;
global using Monolith.Util;

global using System;

global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
EOF

cat > "$PROJECT_DIR/$PROJECT_NAME.csproj" << 'EOF'

<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <RollForward>Major</RollForward>
    <PublishReadyToRun>false</PublishReadyToRun>
    <TieredCompilation>false</TieredCompilation>
  </PropertyGroup>
  <PropertyGroup>
    <ApplicationManifest>app.manifest</ApplicationManifest>
    <ApplicationIcon>Icon.ico</ApplicationIcon>
  </PropertyGroup>
  <ItemGroup>
    <None Remove="Icon.ico" />
    <None Remove="Icon.bmp" />
  </ItemGroup>
  <ItemGroup>
    <EmbeddedResource Include="Icon.ico">
      <LogicalName>Icon.ico</LogicalName>
    </EmbeddedResource>
    <EmbeddedResource Include="Icon.bmp">
      <LogicalName>Icon.bmp</LogicalName>
    </EmbeddedResource>
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="MonoGame.Framework.DesktopGL" Version="3.8.*" />
    <PackageReference Include="MonoGame.Content.Builder.Task" Version="3.8.*" />
  </ItemGroup>
  <ItemGroup>
    <<ProjectReference Include="../Monolith-Engine/Monolith.csproj" />>
  </ItemGroup>
</Project>

EOF

sed -i '' "s/new Game1()/new Main()/" "$PROJECT_DIR/Program.cs" 2>/dev/null || \
sed -i "s/new Game1()/new Main()/" "$PROJECT_DIR/Program.cs"

echo "Custom MonoGame DesktopGL template created at: $PROJECT_DIR"
