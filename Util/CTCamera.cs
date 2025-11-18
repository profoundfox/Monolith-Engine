using System.Numerics;
using System.Runtime.Intrinsics.X86;
using Gum.DataTypes.Variables;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ConstructEngine.Graphics
{
    public class CTCamera
    {
        
        public Vector2 cameraPosition = Vector2.Zero;
        public float Zoom;
        public static CTCamera CurrentCamera;
        public Matrix Transform;
        
        public CTCamera()
        {
            CurrentCamera = this;
        }

        public (Vector2 TopLeft, Vector2 TopRight, Vector2 BottomLeft, Vector2 BottomRight) GetScreenEdges()
        {
            var cfg = Engine.Instance.Config;
            float halfWidth = cfg.RenderWidth / 2f / Zoom;
            float halfHeight = cfg.RenderHeight / 2f / Zoom;

            Vector2 topLeft = new Vector2(cameraPosition.X - halfWidth, cameraPosition.Y - halfHeight);
            Vector2 topRight = new Vector2(cameraPosition.X + halfWidth, cameraPosition.Y - halfHeight);
            Vector2 bottomLeft = new Vector2(cameraPosition.X - halfWidth, cameraPosition.Y + halfHeight);
            Vector2 bottomRight = new Vector2(cameraPosition.X + halfWidth, cameraPosition.Y + halfHeight);

            return (topLeft, topRight, bottomLeft, bottomRight);
        }
        
    }
}