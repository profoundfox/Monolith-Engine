using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Graphics;
using Monolith.Attributes;

namespace Monolith.Nodes
{
    public class AnimatedSprite2D : Node2D
    {
        private int _currentFrame = 0;
        private float _elapsed = 0f;
        private bool _finished = false;

        public Dictionary<string, Animation> Atlas { get; set; } = new Dictionary<string, Animation>();
        public Animation CurrentAnimation { get; private set; }
        public bool IsLooping { get; set; } = false;

        public bool IsFinished => _finished;

        public MTexture CurrentFrame => CurrentAnimation?.Frames[_currentFrame];

        public AnimatedSprite2D() {}

        public void PlayAnimation(string name, bool isLooping = false)
        {
            if (!Atlas.TryGetValue(name, out Animation target))
                return;
            
            if (CurrentAnimation != target || _finished)
            {
                CurrentAnimation = target;
                _currentFrame = 0;
                _elapsed = 0f;
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
                _elapsed = 0f;
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
            _elapsed = 0f;
        }

        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);

            if (_finished || CurrentAnimation == null)
                return;

            _elapsed += delta;

            while (_elapsed >= CurrentAnimation.Delay)
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
                        break;
                    }
                }
            }
        }


        public override void SubmitCall()
        {
            base.SubmitCall();
            
            if (CurrentAnimation == null) return;

            CurrentFrame.Draw
            (
                position: GlobalPosition,
                color: Modulate,
                rotation: Rotation,
                origin: CurrentFrame.Center,
                scale: Scale,
                effects: SpriteEffects,
                shader: Shader,
                depth: Depth
            );
        }
    }
}
