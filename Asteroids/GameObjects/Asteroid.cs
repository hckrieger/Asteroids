using Asteroids.Scenes;
using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroids.GameObjects
{
    class Asteroid : SpriteGameObject
    {

        float baseSpeed = 52.5f, speed, xVelocity, yVelocity;
        public Circle CircleBounds { get; set; }

        public Asteroid() : base("Sprites/Asteroids@3x1", .75f)
        {
            SetOriginToCenter();
            Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            

            xVelocity = (float)Math.Cos(Rotation);
            yVelocity = (float)Math.Sin(Rotation);

            velocity = new Vector2(xVelocity, yVelocity) * speed;

            switch (SheetIndex)
            {
                case 0:
                    speed = baseSpeed;
                    CircleBounds = new Circle(34, LocalPosition);
                    break;
                case 1:
                    speed = baseSpeed * 1.5f;
                    CircleBounds = new Circle(26, LocalPosition);
                    break;
                case 2:
                    speed = baseSpeed * 2;
                    CircleBounds = new Circle(18, LocalPosition);
                    break;
            }

            var projectile = MainScene.ProjectilePool;
            for (int i = 0; i < projectile.Count; ++i)
            {

                if (CollisionDetection.ShapesIntersect(projectile[i].BoundingBox, CircleBounds) && 
                    projectile[i].Active &&
                    projectile[i].CurrentProjectileOrigin == Projectile.ProjectileOrigin.FromShip)
                {

                    ExtendedGame.AssetManager.PlaySoundEffect("Audio/asteroid");
                    switch (SheetIndex)
                    {
                        case 0:
                            SheetIndex = 1;
                            foreach (Asteroid obj in MainScene.AsteroidPool)
                            {
                                if (!obj.Active)
                                {
                                    obj.Active = true;
                                    obj.LocalPosition = GlobalPosition;
                                    obj.Rotation = Rotation + .5f;
                                    obj.SheetIndex = 1;
                                    break;
                                }
                            }
                            Rotation -= .5f;
                            break;
                        case 1:
                            SheetIndex = 2;
                            
                            foreach (Asteroid obj in MainScene.AsteroidPool)
                            {
                                if (!obj.Active)
                                {
                                    obj.Active = true;
                                    obj.LocalPosition = GlobalPosition;
                                    obj.Rotation = Rotation + .5f;
                                    obj.SheetIndex = 2;
                                    break;
                                }

                            }
                            Rotation -= .5f;
                            break;
                        case 2:
                            Active = false;
                            break;
                    }
                    projectile[i].Reset();
                }
                break;
            }

            if (CollisionDetection.ShapesIntersect(MainScene.Ship.CircleBounds, CircleBounds))
            {
                MainScene.Ship.TurnRed();
            }
        }

       

        public override void Reset()
        {
            base.Reset();


            Active = false;
        }

        public MainScene MainScene
        {
            get { return (MainScene)ExtendedGame.GameStateManager.GetGameState(Game1.SCENE_MAIN); }
        }
    }
}
