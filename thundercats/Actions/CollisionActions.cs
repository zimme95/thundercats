﻿using Game_Engine.Components;
using Game_Engine.Entities;
using Game_Engine.Managers;
using System;

namespace thundercats.Actions
{
    static class CollisionActions
    {

        public static void AccelerateColliderForwards(Entity entity, float zDiff)
        {
            
            VelocityComponent velocityComponent = ComponentManager.Instance.GetComponentOfEntity<VelocityComponent>(entity);

            if(velocityComponent != null)
            {
                //velocityComponent.Velocity.Z += 0.25f;
                velocityComponent.Velocity.Z += (0.1f * (Math.Abs(velocityComponent.Velocity.Z) + 1));
            }
        }

        public static void AccelerateColliderBackwards(Entity entity, float zDiff)
        {
            VelocityComponent velocityComponent = ComponentManager.Instance.GetComponentOfEntity<VelocityComponent>(entity);

            if(velocityComponent != null)
            {
                velocityComponent.Velocity.Z -= (0.1f * (Math.Abs(velocityComponent.Velocity.Z) + 1));
            }
        }

        public static void AccelerateColliderLeftwards(Entity entity, float xDiff)
        {
            VelocityComponent velocityComponent = ComponentManager.Instance.GetComponentOfEntity<VelocityComponent>(entity);

            if(velocityComponent != null)
            {
                //velocityComponent.Velocity.X += 0.25f;
                velocityComponent.Velocity.X += (0.1f * (Math.Abs(velocityComponent.Velocity.X) + 1));
            }
        }

        public static void AccelerateColliderRightwards(Entity entity, float xDiff)
        {
            VelocityComponent velocityComponent = ComponentManager.Instance.GetComponentOfEntity<VelocityComponent>(entity);

            if(velocityComponent != null)
            {
                //velocityComponent.Velocity.X -= 0.25f;
                velocityComponent.Velocity.X -= (0.1f * (Math.Abs(velocityComponent.Velocity.X) + 1));
            }
        }

        public static void AccelerateColliderUpwards(Entity entity)
        {
            VelocityComponent velocityComponent = ComponentManager.Instance.GetComponentOfEntity<VelocityComponent>(entity);

            if(velocityComponent != null)
            {
                //velocityComponent.Velocity.Y += 0.1f; //Disabled until smoother adjustment is implemented
            }
        }

        public static void AccelerateColliderDownwards(Entity entity)
        {
            VelocityComponent velocityComponent = ComponentManager.Instance.GetComponentOfEntity<VelocityComponent>(entity);

            if(velocityComponent != null)
            {
                //velocityComponent.Velocity.Y -= 0.1f; //Disabled until smoother adjustment is implemented
            }
        }
    }
}
