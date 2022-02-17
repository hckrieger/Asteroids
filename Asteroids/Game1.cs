using Asteroids.Scenes;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroids
{
    public class Game1 : ExtendedGame
    {
        public const string SCENE_MAIN = "MainScene";
        public const string SCENE_TITLE = "TitleScreen";
        public Game1()
        {
            IsMouseVisible = true;

            windowSize = new Point(768, 576);
            worldSize = new Point(768, 576);
        }


        protected override void LoadContent()
        {
            base.LoadContent();

            // TODO: use this.Content to load your game content here
            GameStateManager.AddGameState(SCENE_TITLE, new StartScreen(worldSize));
            GameStateManager.AddGameState(SCENE_MAIN, new MainScene(worldSize));
            GameStateManager.SwitchTo(SCENE_TITLE);
        }
    }
}
