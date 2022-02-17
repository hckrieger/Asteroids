using Asteroids.GameObjects;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroids.Scenes
{
    class MainScene : GameState
    {
        Ship ship;
        List<Projectile> projectilePool = new List<Projectile>();
        List<Asteroid> asteroidPool = new List<Asteroid>();
        Enemy enemy;
        const int PROJECTILE_QUANTITY = 10;
        const int ASTEROID_QUANTITY = 63;
        Point screenSize;
        SpriteGameObject textBox = new SpriteGameObject("Sprites/TextBox", .99f);
        float playTime, playTimeStart;
        //TextGameObject playTimeDisplay = new TextGameObject("Fonts/Info", 1f, Color.Green, TextGameObject.Alignment.Left);
        TextGameObject header = new TextGameObject("Fonts/Title", 1f, Color.Black, TextGameObject.Alignment.Center);
        TextGameObject info = new TextGameObject("Fonts/Info", 1f, Color.Black, TextGameObject.Alignment.Center);
        TextGameObject playAgain = new TextGameObject("Fonts/Info", 1f, Color.Black, TextGameObject.Alignment.Center);

        State currentState { get; set; } = State.Playing;

        float delayEndTimer, delayEndStart;

        bool hasWon = false;

        public enum State
        {
            Start,
            Playing,
            End
        }

        public MainScene(Point screenSize)
        {
            

            enemy = new Enemy(screenSize);
            ship = new Ship(screenSize);
            //playTimeDisplay.LocalPosition = new Vector2(50, 50);

            header.LocalPosition = new Vector2(screenSize.X/2, 125);
            gameObjects.AddChild(header);

            info.LocalPosition = new Vector2(screenSize.X / 2, 280);
            gameObjects.AddChild(info);

            playAgain.LocalPosition = new Vector2(screenSize.X / 2, 430);
            playAgain.Text = "Press X to play";
            gameObjects.AddChild(playAgain);

            gameObjects.AddChild(ship);
            gameObjects.AddChild(enemy);
            //gameObjects.AddChild(playTimeDisplay);


            textBox.SetOriginToCenter();
            textBox.LocalPosition = new Vector2(screenSize.X, screenSize.Y)/2;
            gameObjects.AddChild(textBox);

            for (int i = 0; i < PROJECTILE_QUANTITY; i++)
            {
                projectilePool.Add(new Projectile(screenSize));
                gameObjects.AddChild(projectilePool[i]);
            }

            for (int i = 0; i < ASTEROID_QUANTITY; i++)
            {
                asteroidPool.Add(new Asteroid());
                gameObjects.AddChild(asteroidPool[i]);
            }

            this.screenSize = screenSize;

            Reset();

            
        }

  

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            if (!AsteroidPool.Exists(m => m.Active))
            {
                if (CurrentState != State.End)
                    ExtendedGame.AssetManager.PlaySong("Audio/win", false);
                CurrentState = State.End;

            }
                

            TransportPosition(ship);

            foreach (Projectile obj in ProjectilePool) 
                TransportPosition(obj);
                
            foreach (Asteroid obj in asteroidPool)
                TransportPosition(obj);






            if (CurrentState == State.Playing)
            {
                
                playTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (CurrentState == State.End)
            {

                if (AsteroidPool.Exists(m => m.Active))
                    ExtendedGame.AssetManager.StopSong();

                delayEndTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                playTime = MathF.Round(playTime);


                if (delayEndTimer <= 0)
                {
                    playAgain.Visible = true;
                    header.Visible = true;
                    info.Visible = true;
                    if (!AsteroidPool.Exists(m => m.Active))
                    {
                        header.Text = "You Win!";
                        info.Text = "Time: " + FormatTime(playTime);
                        
                    } else
                    {
                        header.Text = "Game Over";
                        info.Text = "Try again!";
                    }

                    textBox.Visible = true;

                    ExtendedGame.BackgroundColor = Color.DarkBlue;
                } 
            }


        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (CurrentState == State.End && delayEndTimer <= 0 && inputHelper.KeyPressed(Keys.X))
                Reset();
        }

        //Makes it so sprites can transport back to the opposite end off the screen when it goes off the edge
        void TransportPosition(SpriteGameObject obj)
        {
            if (obj.Active)
            {
                if (obj.LocalPosition.Y <= -obj.Width / 2)
                    obj.LocalPosition = new Vector2(obj.LocalPosition.X, screenSize.Y + obj.Width / 2);
                else if (obj.LocalPosition.Y >= screenSize.Y + obj.Width / 2)
                    obj.LocalPosition = new Vector2(obj.LocalPosition.X, -obj.Width / 2);

                if (obj.LocalPosition.X <= -obj.Width / 2)
                    obj.LocalPosition = new Vector2(screenSize.X + obj.Width / 2, obj.LocalPosition.Y);
                else if (obj.LocalPosition.X >= screenSize.X + obj.Width / 2)
                    obj.LocalPosition = new Vector2(-obj.Width / 2, obj.LocalPosition.Y);
            }

        }

        public List<Projectile> ProjectilePool
        {
            get { return projectilePool; }
            set { projectilePool = value; }
        }

        public List<Asteroid> AsteroidPool
        {
            get { return asteroidPool; }
            set { asteroidPool = value; }
        }

        public Ship Ship => ship;


        public Enemy Enemy => enemy;


        public override void Reset()
        {
            base.Reset();

            
                ExtendedGame.AssetManager.PlaySong("Audio/theme", true);

            delayEndTimer = 2f;
            delayEndStart = delayEndTimer;

            playTime = 0;
            playTimeStart = playTime;

            //playTimeDisplay.Visible = false;

            for (int i = asteroidPool.Count - 1; i >= asteroidPool.Count - 9; i--)
            {
                asteroidPool[i].Active = true;
                asteroidPool[i].SheetIndex = 0;
                asteroidPool[i].Rotation = ExtendedGame.Random.Next(0, 360);
                asteroidPool[i].LocalPosition = new Vector2(ExtendedGame.Random.Next(screenSize.X), -32);
            }

            for (int i = 0; i < asteroidPool.Count - 10; i++)
                asteroidPool[i].Active = false;

            foreach (Projectile obj in projectilePool)
                obj.Reset();

            enemy.Reset();
            ship.Reset();

            ExtendedGame.BackgroundColor = Color.Black;

            currentState = State.Playing;

            textBox.Visible = false;
            header.Visible = false;
            info.Visible = false;
            playAgain.Visible = false;

            playTime = 0;

        }

        public State CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        string FormatTime(float totalSeconds)
        {
            var minutes = Math.Floor(totalSeconds / 60);
            var seconds = Math.Round(totalSeconds % 60, 2);

            string addZero = seconds < 10 ? "0" : ""; 

            string formattedTime = minutes.ToString() + ":" + addZero + seconds.ToString();

            return formattedTime;
        }
    }
}
