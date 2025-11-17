using ConstructEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ConstructEngine.Managers
{
    public enum DrawLayer
    {
        Background,
        Middleground,
        Foreground,
        UI
    }

    public struct DrawCall
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle? SourceRectangle;
        public Color Color;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;
        public SpriteEffects Effects;
        public float LayerDepth;
        public Effect Effect;
    }

    public class DrawManager
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly Dictionary<DrawLayer, List<DrawCall>> _drawQueues;
        private Matrix _cameraTransform = Matrix.Identity;

        public DrawManager(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _drawQueues = new Dictionary<DrawLayer, List<DrawCall>>();

            foreach (DrawLayer layer in Enum.GetValues(typeof(DrawLayer)))
                _drawQueues[layer] = new List<DrawCall>();
        }

        public void SetCamera(Matrix transform)
        {
            _cameraTransform = transform;
        }
        public void Queue(DrawCall drawCall, DrawLayer layer = DrawLayer.Middleground)
        {
            _drawQueues[layer].Add(drawCall);
        }

        public void Draw(
            Texture2D texture,
            Vector2 position,
            Color color,
            DrawLayer layer = DrawLayer.Middleground,
            float rotation = 0f,
            Vector2? origin = null,
            Vector2? scale = null,
            SpriteEffects effects = SpriteEffects.None,
            float layerDepth = 0f,
            Effect effect = null,
            Rectangle? sourceRect = null
        )
        {
            Queue(new DrawCall
            {
                Texture = texture,
                Position = position,
                Color = color,
                Rotation = rotation,
                Origin = origin ?? Vector2.Zero,
                Scale = scale ?? Vector2.One,
                Effects = effects,
                LayerDepth = layerDepth,
                Effect = effect,
                SourceRectangle = sourceRect
            }, layer);
        }
        public void Draw(Sprite sprite, DrawLayer layer = DrawLayer.Middleground)
        {
            Console.WriteLine("yes");
            Queue(new DrawCall
            {
                Texture = sprite.Region.Texture,
                Position = sprite.Position,
                SourceRectangle = sprite.Region.SourceRectangle,
                Color = sprite.Color,
                Rotation = sprite.Rotation,
                Origin = sprite.Origin,
                Scale = sprite.Scale,
                Effects = sprite.Effects,
                LayerDepth = sprite.LayerDepth,
                Effect = null
            }, layer);
        }


        public void Flush()
        {
            foreach (DrawLayer layer in Enum.GetValues(typeof(DrawLayer)))
            {
                
                var queue = _drawQueues[(DrawLayer)layer];
                if (queue.Count == 0) continue;

                _spriteBatch.Begin(
                    SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    transformMatrix: layer == DrawLayer.UI ? Matrix.Identity : _cameraTransform
                );


                foreach (var call in queue)
                {

                    if (call.Texture != null)
                    {   
                        Console.WriteLine($"{call.Texture} {call.Position} {call.SourceRectangle} {call.Color} {call.Rotation} {call.Origin} {call.Scale} {call.Effects} {call.LayerDepth}");
                        _spriteBatch.Draw(
                            call.Texture,
                            call.Position,
                            call.SourceRectangle,
                            call.Color,
                            call.Rotation,
                            call.Origin,
                            call.Scale,
                            call.Effects,
                            call.LayerDepth
                        );
                    }
                }

                _spriteBatch.End();
                queue.Clear();
            }
        }
    }
}
