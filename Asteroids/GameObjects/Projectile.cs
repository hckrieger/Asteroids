using Asteroids.Scenes;
using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroids.GameObjects
{
    class Projectile : SpriteGameObject
    {
        float speed;
        float activeTimer, startTimer, angle;
        bool hasLaunched;
        public Rectangle Bounds { get; set; }
        Vector2 direction;
        Point screenSize;

        public enum ProjectileOrigin
        {
            FromShip,
            FromEnemy,
            None
        }

        public ProjectileOrigin CurrentProjectileOrigin { get; set; }

        public Projectile(Point screenSize) : base("Sprites/projectile", .7f, 0)
        {
            this.screenSize = screenSize;
            activeTimer = 1f;
            startTimer = activeTimer;
            SetOriginToCenter();
            Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Bounds = CustomBounds(new Rectangle(0, 0, 4, 4));

            var ship = MainScene.Ship;
            var enemy = MainScene.Enemy;
            

            if (Active)
            {
                if (CurrentProjectileOrigin == ProjectileOrigin.FromShip)
                {
                    speed = ship.Speed + 350;

                    if (!hasLaunched)
                    {
                        LocalPosition = ship.ProjectilePosition;
                        angle = ship.TotalAngle;
                        hasLaunched = true;
                    }

                    activeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (activeTimer <= 0)
                        Reset();

                    var xVelocity = (float)Math.Cos(angle);
                    var yVelocity = (float)Math.Sin(angle);

                    velocity = new Vector2(xVelocity, yVelocity) * speed;
                }
                else if (CurrentProjectileOrigin == ProjectileOrigin.FromEnemy)
                {
                    speed = 300;

                    if (!hasLaunched)
                    {
                        LocalPosition = enemy.LocalPosition;
                        var distance = MainScene.Ship.GlobalPosition - MainScene.Enemy.GlobalPosition;
                        direction = Vector2.Normalize(distance);
                        hasLaunched = true;
                    }


                    if (GlobalPosition.X < 0 || GlobalPosition.Y < 0 || GlobalPosition.X > screenSize.X || GlobalPosition.Y > screenSize.Y)
                        Reset();

                    velocity = direction * speed;

                    
                }
            } else
            {
                angle = 0;
                LocalPosition = Vector2.Zero - new Vector2(100, 100);
            }

            if (CollisionDetection.ShapesIntersect(BoundingBox, MainScene.Ship.CircleBounds) && CurrentProjectileOrigin == ProjectileOrigin.FromEnemy)
            {
                MainScene.Ship.TurnRed();
                Active = false;
                
            }



        }

        public MainScene MainScene
        {
            get { return (MainScene)ExtendedGame.GameStateManager.GetGameState(Game1.SCENE_MAIN); }
        }

        public override void Reset()
        {
            base.Reset();
            CurrentProjectileOrigin = ProjectileOrigin.None;
            Active = false;
            hasLaunched = false;
            activeTimer = startTimer;
            
        }


    }
}
