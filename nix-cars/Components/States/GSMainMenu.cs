using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nix_cars.Components.GUI;
using nix_cars.Screens;
using SharpDX.Direct3D9;

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
            GumManager.SwitchTo(Screen.MAIN);
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            game.camera.SmoothMoveTo(new Vector3(0,5,-5));
            game.camera.SmoothRotateTo(Vector3.Zero);
            game.camera.Update(uDeltaTimeFloat);

            if (km.KeyDownOnce(km.Escape))
            {
                game.Exit();
            }
            var s = GumManager.GetStartMenu();
            if (km.KeyDownOnce(km.Enter))
            {
                if (s.NameBox.Text == "")
                {
                    s.ToastError.TextInstance.Text = "Ingresa tu nombre para entrar";
                    s.ToastError.IsVisible = true;
                    s.timer.Start();
                }
                else
                {
                    GameStateManager.SwitchTo(State.CARSELECT);
                }
            }

            FinishUpdate();
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            game.GraphicsDevice.SetRenderTarget(null);
            game.GraphicsDevice.Clear(Color.Black);
            game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
;
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
