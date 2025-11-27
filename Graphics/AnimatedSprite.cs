using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Monolith.Graphics
{
    public class AnimatedSprite : Sprite
    {

        private int _currentFrame;
        public TimeSpan _elapsed;
        
        public Animation _animation;
        public bool IsLooping { get; set; } = false;

        public bool finished;

        public Animation Animation
        {
            get => _animation;
            set
            {
                _animation = value;
                Region = _animation.Frames[0];
            }
        }

        public AnimatedSprite() { }

        public AnimatedSprite(Animation animation)
        {
            Animation = animation;
        }


        public bool IsAnimationPlaying()
        {
            return !finished;
        }


        public bool AnimationFinished(Action callback = null)
        {
            if (finished)
            {
                callback?.Invoke();
                return true;
            }

            return false;
        }



        public static async void Wait(float seconds, Action callback)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            callback?.Invoke();
        }



        public void Update(GameTime gameTime)
        {
            if (finished) return;

            _elapsed += gameTime.ElapsedGameTime;

            if (_elapsed >= _animation.Delay)
            {
                _elapsed -= _animation.Delay;
                _currentFrame++;

                if (_currentFrame >= _animation.Frames.Count)
                {
                    if (IsLooping)
                    {
                        _currentFrame = 0;
                    }
                    else
                    {
                        _currentFrame = _animation.Frames.Count - 1;
                        finished = true;
                    }
                }

                Region = _animation.Frames[_currentFrame];
            }
        }



        public void PlayAnimation(Animation animation, bool isLooping)
        {
            if (_animation != animation || finished)
            {
                finished = false;
                _currentFrame = 0;
                _elapsed = TimeSpan.Zero;
                IsLooping = isLooping;
                Animation = animation;
                Region = _animation.Frames[_currentFrame];
            }
        }

        public void ResetAnimation()
        {
            finished = false;
            _currentFrame = 0;
            _elapsed = TimeSpan.Zero;
            Region = _animation.Frames[_currentFrame];
        }

        public void StopAnimation()
        {
            finished = true;
        }

        public void ResumeAnimation()
        {
            finished = false;
        }


        public void SetFrame(int frameIndex)
        {
            if (frameIndex >= 0 && frameIndex < _animation.Frames.Count)
            {
                _currentFrame = frameIndex;
                Region = _animation.Frames[_currentFrame];
            }
        }

        public void GoToFrame(int frameIndex)
        {
            SetFrame(frameIndex);
        }


        public void GoToAndPlay(int frameIndex)
        {
            SetFrame(frameIndex);
            finished = false;


        }

        private async Task WaitForAnimationToFinishAsync()
        {
            while (!finished)
            {
                await Task.Delay(10);
            }
        }


        public async Task PlayAnimationChainAsync(List<Animation> animations, bool isLooping)
        {
            do
            {
                foreach (var animation in animations)
                {
                    PlayAnimation(animation, false);

                    await WaitForAnimationToFinishAsync();
                }
            } 
            while (isLooping);
        }



        


        


}
}