using System;
using System.Collections.Generic;
using ConstructEngine.Components.Physics;
using ConstructEngine.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Components
{
    public interface IKinematicEntity
    {
        void Load();
        void Unload();
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }

    public class KinematicEntity : ConstructObject
    {
        public static List<KinematicEntity> EntityList = new List<KinematicEntity>();
        public KinematicBase KinematicBase;
        public int DamageAmount { get; set; }

        public KinematicEntity(int damageAmount)
        {
            EntityList.Add(this);
            DamageAmount = damageAmount;
            KinematicBase = new KinematicBase(this);
        }

        public override void Load() { }
        public override void Update(GameTime gameTime) { }
        public override void Draw(SpriteBatch spriteBatch) { }
        public override void Unload()
        {
            EntityList.Remove(this);
            KinematicBase.Collider.Free();
        }

        public static void AddEntities(params KinematicEntity[] entities)
        {
            EntityList.AddRange(entities);
        }
    }
}