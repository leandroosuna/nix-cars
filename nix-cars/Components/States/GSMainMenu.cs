using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace nix_cars.Components.States
{
    public class GSMainMenu : GameState
    {
        public GSMainMenu() : base()
        {
        
        }
        public override void OnSwitch()
        {
            game.IsMouseVisible = true;
            mouseLocked = false;
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            //game.camera.RotateBy(new Vector2(uDeltaTimeFloat, 0));

            
            if (km.Escape.IsDown() && !keysDown.Contains(km.Escape))
            {
                game.Exit();
            }

            if (km.Enter.IsDown() && !keysDown.Contains(km.Enter))
            {
                keysDown.Add(km.Enter);

                GameStateManager.SwitchTo(State.RUN);
            }
            FinishUpdate();
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            game.GraphicsDevice.SetRenderTarget(null);
            game.GraphicsDevice.Clear(Color.Black);
            game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //game.skybox.Draw(game.camera.view, game.camera.projection, game.camera.position, false);
            game.spriteBatch.Begin();
            game.spriteBatch.DrawString(game.font25, "Press Enter to start", new Vector2(100, 100), Color.White);
            game.spriteBatch.End();
            

            //gui.Draw(gameTime);
        }
        public override void LostFocus()
        {
            
        }
        public override void Focused()
        {
            
        }

    }
}
