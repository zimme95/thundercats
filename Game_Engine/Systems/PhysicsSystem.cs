﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Game_Engine.Components;
using Game_Engine.Entities;
using Game_Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game_Engine.Systems
{
    /*
     * System to handle all physics updates including 3D transformations based on velocity, friction, and collision.
     * PhysicsSystem uses parallel foreach loops to improve performance, this does not require locks as the component manager is thread safe.
     */
    public class PhysicsSystem : IUpdateableSystem
    {
 
        ComponentManager componentManager = ComponentManager.Instance;

        public void Update(GameTime gameTime)
        {
           //  RunGravity();
            CheckCollision();
            UpdatePositionsOfModels();
        }


        /// <summary>
        /// Updates TransformComponents, ModelComponents, and CollisionComponents with the velocities of any attached VelocityComponent.
        /// </summary>
        private void UpdatePositionsOfModels()
        {
            ConcurrentDictionary<Entity, Component> velocityComponentPairs = componentManager.GetConcurrentDictionary<VelocityComponent>();

            Parallel.ForEach(velocityComponentPairs, velocityComponentPair =>
            {
                VelocityComponent velocityComponent = velocityComponentPair.Value as VelocityComponent;
                TransformComponent transformationComponent = componentManager.ConcurrentGetComponentOfEntity<TransformComponent>(velocityComponentPair.Key);
                ModelComponent modelComponent = componentManager.ConcurrentGetComponentOfEntity<ModelComponent>(velocityComponentPair.Key);

                transformationComponent.Position += velocityComponent.Velocity;
                Matrix translation = Matrix.CreateTranslation(velocityComponent.Velocity.X, velocityComponent.Velocity.Y, velocityComponent.Velocity.Z)
                        * Matrix.CreateRotationX(0) * Matrix.CreateTranslation(velocityComponent.Velocity.X, velocityComponent.Velocity.Y, velocityComponent.Velocity.Z);
                
                if(modelComponent != null)
                {
                    modelComponent.World *= translation;
                }

                UpdatePositionsOfBoundingSpheres(velocityComponentPair.Key, translation);
                UpdateFriction(velocityComponentPair.Key);
            });
        }

       

       /// <summary>
       /// Checks intersections for all BoundingSphereComponents.
       /// Currently only identifies collision, taking action based on collision is TODO.
       /// </summary>
        private void CheckCollision()
        {
            ConcurrentDictionary<Entity, Component> collisionComponentPairs = componentManager.GetConcurrentDictionary<CollisionComponent>();
            bool found = false; //Temp debug flag

            Parallel.ForEach(collisionComponentPairs, sourceCollisionComponentPair =>
            {
                Entity sourceEntity = sourceCollisionComponentPair.Key;
                var sourceCollisionComponent = sourceCollisionComponentPair.Value as CollisionComponent;

                foreach (var targetCollisionComponentPair in collisionComponentPairs)
                {

                    Entity targetEntity = targetCollisionComponentPair.Key;
                    CollisionComponent targetCollisionComponent = targetCollisionComponentPair.Value as CollisionComponent;

                    if(sourceCollisionComponent.ComponentId != targetCollisionComponent.ComponentId &&
                        sourceCollisionComponent.BoundingShape.Intersects(targetCollisionComponent.BoundingShape))
                    {
                        CollisionManager.Instance.AddCollisionPair(sourceEntity, targetEntity);
                        found = true; //Temp debug flag
                        //Console.WriteLine(sourceBoundingSphereComponent.ComponentId.ToString() + " Intersects " + targetBoundingSphereComponent.ComponentId.ToString());
                    }
                }
            });
            if(!found) //Temp debug check
            {
                //Console.WriteLine("No BoundingSphereComponents intersect");
            }
        }


        /// <summary>
        /// Applies gravity to all entities with a gravity-component and velocity-component. 
        /// If they have a velocity-component, but no gravity-component no gravity is applied. 
        /// </summary>
        private void RunGravity(GameTime gameTime)
        {
            //var gravityComponents = ComponentManager.Instance.GetConcurrentDictionary<GravityComponent>();
            //foreach (var gravityComponentKeyValuePair in gravityComponents)
            //{
            //    //if (!(gravityComponentKeyValuePair.Value is GravityComponent)) continue;
            //    var gravityComponent = gravityComponentKeyValuePair.Value as GravityComponent;
            //    var velocityComponent = ComponentManager.Instance.ConcurrentGetComponentOfEntity<VelocityComponent>(gravityComponentKeyValuePair.Key);
            //    var transformComponent = ComponentManager.Instance.ConcurrentGetComponentOfEntity<TransformComponent>(gravityComponentKeyValuePair.Key);
            //    var collisionComponent = ComponentManager.Instance.ConcurrentGetComponentOfEntity<BoundingSphereComponent>(gravityComponentKeyValuePair.Key);

            //    //temp
            //    KeyboardState state = Keyboard.GetState();

            //    // jumps
            //    if (state.IsKeyDown(Keys.Space) && gravityComponent.IsFalling == false)
            //    {
            //        transformComponent.Position = new Vector3(transformComponent.Position.X, transformComponent.Position.Y - 10, transformComponent.Position.Z);
            //        velocityComponent.Velocity.Y = -5;
            //        gravityComponent.IsFalling = true;

            //    }
            //    // falls
            //    if (gravityComponent.IsFalling == true)
            //    {
            //        //velocityComponent.Velocity.Y = +1;
            //        //acceleration = force(time, position) / mass;
            //        //time += timestep;
            //        //position += timestep * velocity;
            //        //velocity += timestep * acceleration;

            //        var dt = gameTime.ElapsedGameTime.Milliseconds;
            //        var acceleration = 9.8f / gravityComponent.Mass;
            //        velocityComponent.Velocity.Y += acceleration * dt;
            //        transformComponent.Position.Y += velocityComponent.Velocity.Y * dt;
            //    }
            //    // if entity has collided with another object
            //    if (transformComponent.Position.Y >= gravityComponent.MinJump) // if y value collides with another object?
            //        gravityComponent.IsFalling = false;

            //    // if is not falling then we do not move vertically
            //    if (gravityComponent.IsFalling == false)
            //        velocityComponent.Velocity.Y = 0;

              //  Console.WriteLine("Player pos: " + transformComponent.Position.ToString()); //For debugging
           // }
        }

        /// <summary>
        /// Updates the positions of bounding spheres of models
        /// BoundingSphereComponents that equal themselves are ignored.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="translation"></param>
        private void UpdatePositionsOfBoundingSpheres(Entity key, Matrix translation)
        {
            CollisionComponent collisionComponent = componentManager.ConcurrentGetComponentOfEntity<CollisionComponent>(key);

            if (collisionComponent != null)
            {
                var boundingSphere = collisionComponent.BoundingShape;
                boundingSphere = collisionComponent.BoundingShape.Transform(translation);
                collisionComponent.BoundingShape = boundingSphere;
            }
        }

        /// <summary>
        /// Updates the friction.
        /// </summary>
        /// <param name="velocityComponent"></param>
        private void UpdateFriction(Entity key)
        {
            var velocityComponent = componentManager.ConcurrentGetComponentOfEntity<VelocityComponent>(key);
            var frictionComponent = componentManager.ConcurrentGetComponentOfEntity<FrictionComponent>(key);
            // Placeholder friction
            if(frictionComponent != null)
            {
                velocityComponent.Velocity.X *= frictionComponent.Friction;
                velocityComponent.Velocity.Z *= frictionComponent.Friction;
            }
        }

        /*
         * Translates a model component to be at the same world position as a transform component.
         */
        public static void SetInitialModelPos(ModelComponent modelComponent, TransformComponent transformComponent)
        {
            modelComponent.World = Matrix.CreateTranslation(transformComponent.Position.X, transformComponent.Position.Y, transformComponent.Position.Z);
        }

        /*
         * Translates a bounding sphere component to be at the same world position as a transform component.
         */
        public static void SetInitialBoundingSpherePos(CollisionComponent collisionComponent, TransformComponent transformComponent)
        {
            Matrix translation = Matrix.CreateTranslation(transformComponent.Position.X, transformComponent.Position.Y, transformComponent.Position.Z);
            var boundingSphere = collisionComponent.BoundingShape;
            boundingSphere = collisionComponent.BoundingShape.Transform(translation);
            collisionComponent.BoundingShape = boundingSphere;
        }

        public static void SetInitialBoundingBox(CollisionComponent collisionComponent, TransformComponent transformComponent)
        {
            var boundingBox = collisionComponent.BoundingShape;
            var lengthX = (boundingBox.Max.X - boundingBox.Min.X) /2;
            var lengthY = (boundingBox.Max.Y - boundingBox.Min.Y) /2;
            var lengthZ = (boundingBox.Max.Z - boundingBox.Min.Z) /2;

            var min = new Vector3(transformComponent.Position.X - lengthX, transformComponent.Position.Y - lengthY, transformComponent.Position.Z - lengthZ);
            var max = new Vector3(transformComponent.Position.X + lengthX, transformComponent.Position.Y + lengthY, transformComponent.Position.Z + lengthZ);
            collisionComponent.BoundingShape = new BoundingBox(min, max);
        }
    }
}
