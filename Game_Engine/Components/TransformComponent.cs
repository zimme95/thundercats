﻿using Game_Engine.Entities;
using Game_Engine.Helpers;
using Microsoft.Xna.Framework;

namespace Game_Engine.Components
{
    public class TransformComponent : Component
    {
        public Vector3 Position;
        public Vector3 Scale { get; set; }
        public Matrix RotationMatrix { get; set; }

        public TransformComponent(Entity id) : base(id)
        {
            Position = new Vector3(0, 0, 0);
        }

        public TransformComponent(Entity id, Vector3 pos) : base(id)
        {
            Position = pos;
        }
    }
}
