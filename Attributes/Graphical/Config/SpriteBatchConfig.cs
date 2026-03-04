using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Attributes
{
    public readonly record struct SpriteBatchConfig : IEquatable<SpriteBatchConfig>
    {
        public SpriteSortMode SortMode { get; init; }
        public BlendState BlendState { get; init; }
        public SamplerState SamplerState { get; init; }
        public DepthStencilState DepthStencilState { get; init; }
        public RasterizerState RasterizerState { get; init; }
        public Effect Effect { get; init; }

        public static SpriteBatchConfig Default => new()
        {
            SortMode = SpriteSortMode.BackToFront,
            BlendState = BlendState.AlphaBlend,
            SamplerState = SamplerState.PointClamp,
            DepthStencilState = DepthStencilState.None,
            RasterizerState = RasterizerState.CullCounterClockwise,
            Effect = null
        };

        public bool Equals(SpriteBatchConfig other)
        {
            return SortMode == other.SortMode
                && ReferenceEquals(BlendState, other.BlendState)
                && ReferenceEquals(SamplerState, other.SamplerState)
                && ReferenceEquals(DepthStencilState, other.DepthStencilState)
                && ReferenceEquals(RasterizerState, other.RasterizerState)
                && ReferenceEquals(Effect, other.Effect);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)SortMode;
                hash = (hash * 397) ^ (BlendState != null ? RuntimeHelpers.GetHashCode(BlendState) : 0);
                hash = (hash * 397) ^ (SamplerState != null ? RuntimeHelpers.GetHashCode(SamplerState) : 0);
                hash = (hash * 397) ^ (DepthStencilState != null ? RuntimeHelpers.GetHashCode(DepthStencilState) : 0);
                hash = (hash * 397) ^ (RasterizerState != null ? RuntimeHelpers.GetHashCode(RasterizerState) : 0);
                hash = (hash * 397) ^ (Effect != null ? RuntimeHelpers.GetHashCode(Effect) : 0);
                return hash;
            }
}
    }
}