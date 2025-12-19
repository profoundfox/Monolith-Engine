using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;

namespace Monolith.Nodes
{
    public record class AnimatedSpriteConfig : SpriteConfig
    {
        public Dictionary<string, Animation> Atlas { get; set; } = new Dictionary<string, Animation>();
        public bool IsLooping { get; set; } = false;
    }

    public class AnimatedSprite2D : Node2D
    {
        private int _currentFrame = 0;
        private TimeSpan _elapsed = TimeSpan.Zero;
        private bool _finished = false;

        public Dictionary<string, Animation> Atlas { get; set; }
        public Animation CurrentAnimation { get; private set; }
        public bool IsLooping { get; private set; } = false;

        public MTexture CurrentFrame => CurrentAnimation?.Frames[_currentFrame];

        public Color Modulate { get; set; } = Color.White;
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0f;
        public float LayerDepth { get; set; }

        public SpriteEffects Effects { get; set; }

        public bool Finished => _finished;


        public AnimatedSprite2D(AnimatedSpriteConfig cfg) : base(cfg)
        {
            Atlas = cfg.Atlas;
            IsLooping = cfg.IsLooping;
            Modulate = cfg.Modulate;
            Scale = cfg.Scale;
            Rotation = cfg.Rotation;
        }

        public void PlayAnimation(string name, bool isLooping = false)
        {
            if (!Atlas.TryGetValue(name, out Animation target))
                return;

            if (CurrentAnimation != target || _finished)
            {
                CurrentAnimation = target;
                _currentFrame = 0;
                _elapsed = TimeSpan.Zero;
                _finished = false;
                IsLooping = isLooping;
            }
        }

        public void PlayAnimation(Animation animation, bool isLooping = false)
        {
            if (animation == null)
                return;
            
            if (CurrentAnimation != animation || _finished)
            {
                CurrentAnimation = animation;
                _currentFrame = 0;
                _elapsed = TimeSpan.Zero;
                _finished = false;
                IsLooping = isLooping;
            }
        }

        public void StopAnimation()
        {
            _finished = true;
        }

        public void ResetAnimation()
        {
            _finished = false;
            _currentFrame = 0;
            _elapsed = TimeSpan.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            if (_finished || CurrentAnimation == null) return;

            _elapsed += gameTime.ElapsedGameTime;

            if (_elapsed >= CurrentAnimation.Delay)
            {
                _elapsed -= CurrentAnimation.Delay;
                _currentFrame++;

                if (_currentFrame >= CurrentAnimation.Frames.Count)
                {
                    if (IsLooping)
                    {
                        _currentFrame = 0;
                    }
                    else
                    {
                        _currentFrame = CurrentAnimation.Frames.Count - 1;
                        _finished = true;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentAnimation == null) return;

            CurrentFrame.Draw(
                position: GlobalTransform.Position,
                color: Modulate,
                rotation: Rotation,
                origin: CurrentFrame.Center,
                scale: Scale,
                effects: Effects,
                layerDepth: LayerDepth
            );
        }
    }
}
