using Asteroids.Scenes;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroids.GameObjects
{
    class Ship : SpriteGameObject
    {
        float moveSpeed, rotationOffset = 44.768f, rotationSpeed = 0, accelerator = 0;
        float xVelocity, yVelocity;
        public Circle CircleBounds { get; set; }
        public float SecondsStill { get; set; }
        public float startSecondsStill;
        Point screenSize;



        enum ShipState
        {
            Accelerate,
            Decelerate,
            Still
        }

        ShipState currentShipState;

        public float TotalAngle => Rotation - rotationOffset; 
      

        public Ship(Point screenSize) : base("Sprites/Ship@2x1", .5f)
        {
            this.screenSize = screenSize;
            Reset();
            SetOriginToCenter();
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (MainScene.CurrentState == MainScene.State.Playing && Color == Color.White)
            {
                if (inputHelper.KeyDown(Keys.Left))
                { rotationSpeed = -2.5f; }
                else if (inputHelper.KeyDown(Keys.Right))
                { rotationSpeed = 2.5f; }
                else
                { rotationSpeed = 0; }

                if (inputHelper.KeyDown(Keys.Up))
                {
                    currentShipState = ShipState.Accelerate;
                    SheetIndex = 1;
                }
                else
                {
                    if (accelerator > 0)
                        currentShipState = ShipState.Decelerate;
                    else if (accelerator <= 0)
                        currentShipState = ShipState.Still;

                    SheetIndex = 0;
                }

                if (inputHelper.KeyPressed(Keys.X))
                {
                    ExtendedGame.AssetManager.PlaySoundEffect("Audio/fire");
                    foreach (Projectile obj in MainScene.ProjectilePool)
                    {
                        if (!obj.Active)
                        {
                            obj.Active = true;
                            obj.CurrentProjectileOrigin = Projectile.ProjectileOrigin.FromShip;
                            break;
                        }
                    }
                }
            }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            
                

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            CircleBounds = new Circle(10, GlobalPosition);

            switch (currentShipState)
            {
                case (ShipState.Still):
                    if (!MainScene.Enemy.Active)
                        SecondsStill -= dt;

                    if (SecondsStill <= 0 && MainScene.CurrentState == MainScene.State.Playing)
                    {
                        if (GlobalPosition.Y <= screenSize.Y / 2)
                            MainScene.Enemy.Launch(new Vector2(screenSize.X + MainScene.Enemy.Width, screenSize.Y / 1.333f));
                        else if (GlobalPosition.Y > screenSize.Y / 2)
                            MainScene.Enemy.Launch(new Vector2(-MainScene.Enemy.Width, screenSize.Y / 4f));

                        SecondsStill = startSecondsStill;

                    }
                    accelerator = 0;
                    break;
                case (ShipState.Accelerate):
                    SecondsStill = startSecondsStill;
                    xVelocity = (float)Math.Cos(TotalAngle);
                    yVelocity = (float)Math.Sin(TotalAngle);
                    if (accelerator <= 3f)
                        accelerator += dt * .85f;
                    break;
                case (ShipState.Decelerate):
                    if (accelerator < 1)
                        accelerator -= dt * .5f;
                    else
                        accelerator -= dt * 1.5f;
                    break;
                default:
                    accelerator = 0;
                    break;
            }


            if (MainScene.CurrentState == MainScene.State.Playing && Color == Color.White)
            {
                Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * rotationSpeed;
                velocity = new Vector2(xVelocity, yVelocity) * accelerator * moveSpeed;
            } else
            {
                velocity = Vector2.Zero;
            }

            if (CollisionDetection.ShapesIntersect(MainScene.Enemy.CircleBounds, CircleBounds))
            {
                ExtendedGame.BackgroundColor = Color.Green;
            }
        }


        public MainScene MainScene
        {
            get { return (MainScene)ExtendedGame.GameStateManager.GetGameState(Game1.SCENE_MAIN); }
        }

        public Vector2 ProjectilePosition
        {
            get
            {
                float yPosition = (float)Math.Sin(TotalAngle) * Width * .8f;
                float xPosition = (float)Math.Cos(TotalAngle) * Width * .8f;
                return GlobalPosition + new Vector2(xPosition, yPosition);
            }
        }


        public void TurnRed()
        {
            if (Color == Color.White)
                ExtendedGame.AssetManager.PlaySoundEffect("Audio/gameover");
            Color = Color.Red;
            MainScene.CurrentState = MainScene.State.End;


        }
            


        public override void Reset()
        {
            base.Reset();
            Color = Color.White;
            moveSpeed = 200;
            currentShipState = ShipState.Still;
            LocalPosition = new Vector2(screenSize.X/2, screenSize.Y/2);
            SecondsStill = 7f;
            startSecondsStill = SecondsStill;
            Rotation = rotationOffset;
        }


        public float Speed
        {
            get { return moveSpeed * accelerator; }
        }
    }
}
