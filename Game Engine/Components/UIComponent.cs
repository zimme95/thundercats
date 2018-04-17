﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Engine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game_Engine.Components
{
    public class UIComponent : Component
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public SpriteFont SpriteFont { get; set; }
        public Texture2D UITexture { get; set; }
        public string Text { get; set; } 
        public Color Color { get; set; } = Color.White; 
                
        public UIComponent(Entity id) : base(id)
        {
        }

    }
}   
