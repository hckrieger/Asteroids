using Asteroids.Scenes;
using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroids.GameObjects
{
    class Enemy : SpriteGameObject
    {
        float shotTimer, startShotTimer;
        Point screenSize;
        float speed = 100;
        public Circle CircleBounds { get; set; }

        public Enemy(Point screenSize) : base("Sprites/Enemy", .7f)
        {
            this.screenSize = screenSize;
            SetOriginToCenter();
            Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CircleBounds = new Circle(10, GlobalPosition);

            if (Active)
            {
                if (GlobalPosition.Y > screenSize.Y / 2)
                    velocity = new Vector2(-speed, 0);
                else
                    velocity = new Vector2(speed, 0);

                var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (MainScene.CurrentState == MainScene.State.Playing)
                    shotTimer -= dt;

                if (shotTimer <= 0)
                {
                   
                    ExtendedGame.AssetManager.PlaySoundEffect("Audio/fire");
                    foreach(Projectile obj in MainScene.ProjectilePool)
                    {
                        if (!obj.Active)
                        {
                            
                            obj.Active = true;
                            obj.CurrentProjectileOrigin = Projectile.ProjectileOrigin.FromEnemy;
                            break;
                        }
                    }

                    shotTimer = startShotTimer;
                }

                if ((GlobalPosition.X > screenSize.X + 50) || (GlobalPosition.X < -50))
                {
                    if (Active)
                        ExtendedGame.AssetManager.PlaySong("Audio/theme", true);
                    Active = false;
                }
                    

            } 
        }

        public void Launch(Vector2 startingPosition)
        {
            if (!Active) 
            {
                ExtendedGame.AssetManager.PlaySong("Audio/enemy", true);
            } 
                

            Active = true;
            LocalPosition = startingPosition;
        }


        public void Pause()
        {
            if (Active)
                velocity = Vector2.Zero;
        }

        public override void Reset()
        {
            base.Reset();
            Active = false;
            shotTimer = 2f;
            startShotTimer = shotTimer;
        }

        public MainScene MainScene
        {
            get { return (MainScene)ExtendedGame.GameStateManager.GetGameState(Game1.SCENE_MAIN); }
        }
    }
}
