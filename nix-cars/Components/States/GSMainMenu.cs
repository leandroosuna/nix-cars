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

            if(km.KeyDownOnce(km.Escape))
            {
                game.Exit();
            }

            if(km.KeyDownOnce(km.Enter))
            {
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
            

            FinishDraw();
            //gui.Draw(gameTime);
        }
        public override void OnResolutionChange(int w, int h)
        {

        }
        public override void LostFocus()
        {
            
        }
        public override void Focused()
        {
            
        }

    }
}
