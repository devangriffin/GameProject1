﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject1
{
    public class Coin
    {
        private Texture2D texture;
        private double animationTimer;
        private short animationFrame = 1;
        private Vector2 position;
        private Vector2 origin = new Vector2(16, 16);

        private const float RADIUS = 16;

        /// <summary>
        /// Constructor for a coin
        /// </summary>
        public Coin()
        {
            Random random = new Random();
            position = new Vector2(random.NextInt64(800), random.NextInt64(480));
        }

        /// <summary>
        /// Loads the texture
        /// </summary>
        /// <param name="content">The content manager</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("CoinSprite");
        }

        /// <summary>
        /// Draws the sprite
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The sprite batch</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            
            if (animationTimer > 0.2)
            {
                animationFrame++;
                if (animationFrame > 8) { animationFrame = 1;  }
                animationTimer -= 0.2;
            }
            
            Rectangle rect;
            switch (animationFrame)
            {
                case 1: // Red
                    rect = new Rectangle(0, 0, 32, 32);
                    break;
                case 2: // Orange
                    rect = new Rectangle(32, 0, 32, 32);
                    break;
                case 3: // Yellow
                    rect = new Rectangle(64, 0, 32, 32);
                    break;
                case 4: // Green
                    rect = new Rectangle(0, 32, 32, 32);
                    break;
                case 5: // Blue
                    rect = new Rectangle(32, 32, 32, 32);
                    break;
                case 6: // Indigo
                    rect = new Rectangle(64, 32, 32, 32);
                    break;
                case 7: // Violet
                    rect = new Rectangle(0, 64, 32, 32);
                    break;
                case 8: // Pink
                    rect = new Rectangle(32, 64, 32, 32);
                    break;
                default:
                    rect = new Rectangle();
                    break;
            }

            spriteBatch.Draw(texture, position, rect, Color.White);
        }
    }
}
