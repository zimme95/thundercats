﻿using Game_Engine.Entities;
using Microsoft.Xna.Framework;
namespace Game_Engine.Components
{
   public class CameraComponent : Component
    {
        /*Properties*/
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public Matrix WorldMatrix { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }
        public float FieldOfView { get; set; }
        public float AspectRatio { get; set; }

        public CameraComponent(Entity id) : base(id)
        {
            WorldMatrix = new Matrix();
            ViewMatrix = new Matrix();
            ProjectionMatrix = new Matrix();
            FieldOfView = MathHelper.PiOver2;
            Target = Vector3.Zero;
            Position = Vector3.Zero;
        }

        public CameraComponent(Entity id, Vector3 position, float aspectRatio) : this(id)
        {
            WorldMatrix = new Matrix();
            ViewMatrix = new Matrix();
            ProjectionMatrix = new Matrix();
            FieldOfView = MathHelper.PiOver2;
            Target = Vector3.Zero;
            Position = position;
            AspectRatio = aspectRatio;
        }
    }
}
