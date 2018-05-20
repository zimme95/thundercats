﻿using Game_Engine.Components;
using Game_Engine.Entities;
using Game_Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using thundercats.Factory;
using System;
using System.Linq;
using Game_Engine.Systems;
using System.Collections.Generic;
using thundercats.Service;
using thundercats.Systems;
using thundercats.Components;

namespace thundercats.GameStates.States.PlayingStates
{
    public class PlayingLocalGame : IPlaying
    {
        private GameManager gameManager;
        private Viewport viewport;
        internal WorldGenerator worldGenerator;

        private ParticleSystem particleSystem;
        private ParticleCreationSystem particleCreationSystem;

        public PlayingLocalGame(GameManager gameManager)
        {
            this.gameManager = gameManager;
            viewport = gameManager.game.GraphicsDevice.Viewport;
        }

        public void Initialize()
        {   
            particleSystem = new ParticleSystem(gameManager.game.GraphicsDevice);
            particleCreationSystem = new ParticleCreationSystem(particleSystem);
            SystemManager.Instance.AddToDrawables(particleSystem);
            SystemManager.Instance.AddToUpdateables(particleSystem, particleCreationSystem);

            var playerEntity = GameEntityFactory.NewLocalPlayer("Models/Blob", 0, new Vector3(0, 100, -5),
                new Vector3(0, 500, -100), viewport.AspectRatio, true,
                AssetManager.Instance.CreateTexture(Color.Red, gameManager.game.GraphicsDevice));
            //GameEntityFactory.NewParticleSettingsEntity(playerEntity, 100, 2, "fire");
            GameEntityFactory.NewParticleSettingsEntity(playerEntity, 100, 2, "smoke");

            //GameEntityFactory.NewAiPlayer("Models/Blob", 0, new Vector3(0, -10, 0),
            //    AssetManager.Instance.CreateTexture(Color.Honeydew, gameManager.game.GraphicsDevice));

            particleSystem.InitializeParticleSystem(ComponentManager.Instance.GetComponentOfEntity<ParticleSettingsComponent>(playerEntity));
            InitWorld();

            AudioManager.Instance.ClearSongs();
            AudioManager.Instance.EnqueueSongs("playMusic1", "playMusic2");
        }
       
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(AssetManager.Instance.GetContent<Texture2D>("2DTextures/stars"), viewport.TitleSafeArea, Color.White);
            spriteBatch.End();
            SystemManager.Instance.Draw(gameTime);
        }

        public void Update(GameTime gameTime)
        {
            if(!AudioManager.Instance.IsPlaying)
            {
                AudioManager.Instance.PlayNextInQueue(gameTime);
            }
            SystemManager.Instance.Update(gameTime);
        }

        /// <summary>
        /// Initiates the gameworld by generating a world matrix.
        /// Creates blocks and places them on the positions in the world matrix. 
        /// </summary>
        private void InitWorld()
        {
            worldGenerator = new WorldGenerator("Somebody once told me the wolrd is gonna roll me", WorldGenerator.GetWorldgenEntityDefs(), gameManager, viewport);
            GenerateWorld(3, 20);
            worldGenerator.MoveBlocks();
        }

        private void GenerateWorld(int nLanes, int nRows)
        {
            worldGenerator.GenerateWorld(nLanes, nRows);
        }
    }
}
