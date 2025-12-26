using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Graphics
{
    public readonly struct SpriteBatchConfig : IEquatable<SpriteBatchConfig>
    {
        public SpriteSortMode SortMode { get; }
        public BlendState BlendState { get; }
        public SamplerState SamplerState { get; }
        public DepthStencilState DepthStencilState { get; }
        public RasterizerState RasterizerState { get; }
        public Effect Effect { get; }

        public SpriteBatchConfig(
            SpriteSortMode sortMode = SpriteSortMode.Deferred,
            BlendState blendState = null,
            SamplerState samplerState = null,
            DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null,
            Effect effect = null)
        {
            SortMode = sortMode;
            BlendState = blendState;
            SamplerState = samplerState;
            DepthStencilState = depthStencilState;
            RasterizerState = rasterizerState;
            Effect = effect;

        }

        public bool Equals(SpriteBatchConfig other)
        {
            return SortMode == other.SortMode
                && ReferenceEquals(BlendState, other.BlendState)
                && ReferenceEquals(SamplerState, other.SamplerState)
                && ReferenceEquals(DepthStencilState, other.DepthStencilState)
                && ReferenceEquals(RasterizerState, other.RasterizerState)
                && ReferenceEquals(Effect, other.Effect);
        }

        public override bool Equals(object obj)
            => obj is SpriteBatchConfig other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)SortMode;
                hash = (hash * 397) ^ (BlendState?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ (SamplerState?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ (DepthStencilState?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ (RasterizerState?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ (Effect?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public static SpriteBatchConfig Default => new SpriteBatchConfig(
            sortMode: SpriteSortMode.BackToFront,
            blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.PointClamp,
            depthStencilState: DepthStencilState.None,
            rasterizerState: RasterizerState.CullCounterClockwise
        );
    }
}