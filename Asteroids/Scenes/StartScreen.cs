using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Asteroids.Scenes
{
    class StartScreen : GameState
    {
        TextGameObject title = new TextGameObject("Fonts/Title", 1f, Color.White, TextGameObject.Alignment.Center);
        TextGameObject instructions = new TextGameObject("Fonts/Instructions", 1f, Color.White, TextGameObject.Alignment.Center);
        TextGameObject press = new TextGameObject("Fonts/Instructions", 1f, Color.White, TextGameObject.Alignment.Center);
        TextGameObject credit = new TextGameObject("Fonts/Info", 1f, Color.White, TextGameObject.Alignment.Center);

        float timer, startTimer;

        public StartScreen(Point worldSize)
        {

            timer = .85f;
            startTimer = timer;

            gameObjects.AddChild(instructions);
            instructions.LocalPosition = new Vector2(worldSize.X / 2, 200);
            instructions.Text = "                  Instructions:\n\n" +
                                "Rotate - Left and Right Arrow Keys\n\n" +
                                "Accelerate - Up Arrow Key\n\n" +
                                "Shoot Projectile - X Key";


            gameObjects.AddChild(title);
            title.Text = "Asteroids";
            title.LocalPosition = new Vector2(worldSize.X / 2, 75);

            gameObjects.AddChild(press);
            press.Text = "Press X to start";
            press.LocalPosition = new Vector2(worldSize.X / 2, 425);

            gameObjects.AddChild(credit);
            credit.Text = "Programmed by Hunter Krieger";
            credit.LocalPosition = new Vector2(worldSize.X / 2, 515);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (timer <= 0)
            {
                if (press.Visible)
                    press.Visible = false;
                else
                    press.Visible = true;

                timer = startTimer;
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            base.HandleInput(inputHelper);

            if (inputHelper.KeyPressed(Keys.X))
            {
                ExtendedGame.GameStateManager.SwitchTo(Game1.SCENE_MAIN);
            }
        }
    }
}
