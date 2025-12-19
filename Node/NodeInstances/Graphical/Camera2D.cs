using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Util;

namespace Monolith.Nodes
{
    public record class CameraConfig : SpatialNodeConfig
    {        
        public float Zoom { get; set; } = 1f;
    }
    public class Camera2D : Node2D 
    {
        public float Zoom { get; set; }

        public MCamera InternalCamera { get; set; }

        public Camera2D(CameraConfig cfg) : base(cfg)
        {
            Zoom = cfg.Zoom;
        }

        public override void Load()
        {
            base.Load();

            InternalCamera = new MCamera();

            InternalCamera.Zoom = Zoom;
            InternalCamera.Position = Position;

            PositionChanged += (newPos) =>
            {
                InternalCamera.Position = newPos;
            };
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            InternalCamera.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}