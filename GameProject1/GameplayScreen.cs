﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Threading;
using ParticleSystemExample;
using GameProject1.Content;

namespace GameProject1
{
    public class GameplayScreen
    {
        private GraphicsDeviceManager graphics;

        private List<Coin> coins;
        private List<Alien> aliens;
        private CueBall cueBall;
        private BoxCharacter boxMan;

        private Texture2D background;
        private SpriteFont font;
        private SoundEffect coinPickup;
        private SoundEffect bounce;
        private Firework firework;

        private bool Colliding = false;
        private bool CueColliding = false;
        private bool GameOver = false;

        private int coinsCollected = 0;
        private int endAmount;
        private int CoinCount = 3;
        private int alienCount = 4;
        private int level = 1;

        public float seconds = 0;
        public int minutes = 0;       

        private GameProject1 game;

        public GameplayScreen(GraphicsDeviceManager graphics, SpriteFont font)
        {
            this.graphics = graphics;
            this.font = font;
        }

        public void Initialize(int endAmount, GameProject1 game)
        {
            boxMan = new BoxCharacter();
            coins = new List<Coin>();
            aliens = new List<Alien>();
            for (int i = 0; i < CoinCount; i++) { coins.Add(new Coin()); }
            for (int i = 0; i < alienCount; i++) { aliens.Add(new Alien(boxMan.Position)); }
            cueBall = new CueBall(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            this.endAmount = endAmount;
            this.game = game;

            firework = new Firework(game, 40);
            game.Components.Add(firework);
        }

        public void Load(ContentManager c)
        {
            boxMan.LoadContent(c);
            foreach (Coin coin in coins) { coin.LoadContent(c); }
            foreach (Alien alien in aliens) { alien.LoadContent(c); }
            cueBall.LoadContent(c);

            background = c.Load<Texture2D>("Space1");
            coinPickup = c.Load<SoundEffect>("coinPickup");
            bounce = c.Load<SoundEffect>("bounce");
            // music = c.Load<Song>("SpaceMusic");
            // MediaPlayer.IsRepeating = true;
            // MediaPlayer.Play(music);
        }

        public void Update(GameTime gameTime)
        {
            boxMan.Update(gameTime);

            foreach (Alien alien in aliens)
            {
                alien.Update(boxMan.Position);
                if (cueBall.HitBox.Collides(alien.HitBox))
                {
                    if (alien.HitBox.IsColliding == false)
                    {
                        Bounce(cueBall.HitBox, boxMan.HitBox);
                        alien.NewPosition(boxMan.Position);
                    }

                    alien.HitBox.IsColliding = true;
                }
                else { alien.HitBox.IsColliding = false; }
            }
            

            if (!GameOver) { cueBall.Update(gameTime, bounce); }        

            if (cueBall.HitBox.Collides(boxMan.HitBox))
            {
                Colliding = true;

                // Makes sure it doesn't bounce multiple times while colliding
                if (CueColliding == false)
                {
                    bounce.Play();

                    /*
                    float DistanceToX = 0;
                    float DistanceToY = 0;

                    // Decides which direction the ball goes after bouncing off the character
                    if (cueBall.HitBox.Center.X < boxMan.HitBox.X) { DistanceToX = boxMan.HitBox.X - cueBall.HitBox.Center.X; }
                    else if (cueBall.HitBox.Center.X > boxMan.HitBox.X + boxMan.HitBox.Width) { DistanceToX = cueBall.HitBox.Center.X - (boxMan.HitBox.X + boxMan.HitBox.Width); }
                    if (cueBall.HitBox.Center.Y < boxMan.HitBox.Y) { DistanceToY = boxMan.HitBox.Y - cueBall.HitBox.Center.Y; }
                    else if (cueBall.HitBox.Center.Y > boxMan.HitBox.Y + boxMan.HitBox.Height) { DistanceToY = cueBall.HitBox.Center.Y - (boxMan.HitBox.Y + boxMan.HitBox.Height); }

                    if (DistanceToY > DistanceToX) { cueBall.Velocity.Y *= -1; }
                    else { cueBall.Velocity.X *= -1; }
                    */

                    Bounce(cueBall.HitBox, boxMan.HitBox);

                    CueColliding = true;
                }
            }
            else
            {
                Colliding = false;
                CueColliding = false;
            }

            foreach (Coin coin in coins)
            {
                if (cueBall.HitBox.Collides(coin.HitBox))
                {
                    coinPickup.Play();
                    firework.PlaceFirework(coin.Position);
                    coin.MoveCoin();
                    coinsCollected++;
                }
            }
        }

        public bool Draw(GameTime gameTime, SpriteBatch sb)
        {
            seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (seconds > 60) 
            { 
                minutes++;
                seconds -= 60;            
            }
            
            if (coinsCollected < endAmount)
            {
                //sb.Draw(background, new Rectangle(300, 300, 400, 300), new Rectangle(0, 0, 300, 300), Color.White);
                sb.Draw(background, new Vector2(0, 0), Color.White);
                boxMan.Draw(gameTime, sb, Colliding);
                foreach (Coin coin in coins) { coin.Draw(gameTime, sb); }
                foreach (Alien alien in aliens) { alien.Draw(sb); } 
                cueBall.Draw(gameTime, sb);

                sb.DrawString(font, "Coins Collected: " + coinsCollected, new Vector2(0, 0), Color.Gold);
                if (seconds < 10) { sb.DrawString(font, "Time: " + minutes + ":0" + (int)seconds, new Vector2(0, graphics.PreferredBackBufferHeight - 30), Color.Gold); }
                else { sb.DrawString(font, "Time: " + minutes + ":" + (int)seconds, new Vector2(0, graphics.PreferredBackBufferHeight - 30), Color.Gold); }
                sb.DrawString(font, "Level " + level, new Vector2(graphics.PreferredBackBufferWidth - 60, 0), Color.Gold);

                return true;
            }
            else
            {
                if ((game.RecordMinutes * 60) + game.RecordSeconds > (minutes * 60) + seconds || game.RecordMinutes == -1)
                {
                    game.RecordMinutes = minutes;
                    game.RecordSeconds = (int)seconds;
                }
                return false;
            }
        }

        public void ResetGamePlay()
        {
            coinsCollected = 0;
            minutes = 0;
            seconds = 0;
        }

        private void Bounce(BoundingCircle cueBallHitBox, BoundingRectangle squareHitBox)
        {
            float DistanceToX = 0;
            float DistanceToY = 0;

            // Decides which direction the ball goes after bouncing off the character
            if (cueBallHitBox.Center.X < squareHitBox.X) { DistanceToX = squareHitBox.X - cueBallHitBox.Center.X; }
            else if (cueBallHitBox.Center.X > squareHitBox.X + squareHitBox.Width) { DistanceToX = cueBallHitBox.Center.X - (squareHitBox.X + squareHitBox.Width); }
            if (cueBallHitBox.Center.Y < squareHitBox.Y) { DistanceToY = squareHitBox.Y - cueBallHitBox.Center.Y; }
            else if (cueBallHitBox.Center.Y > squareHitBox.Y + squareHitBox.Height) { DistanceToY = cueBallHitBox.Center.Y - (squareHitBox.Y + squareHitBox.Height); }

            if (DistanceToY > DistanceToX) { cueBall.Velocity.Y *= -1; }
            else { cueBall.Velocity.X *= -1; }
        }
    }
}
